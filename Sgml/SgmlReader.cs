/*
* 
* An XmlReader implementation for loading SGML (including HTML) converting it
* to well formed XML, by adding missing quotes, empty attribute values, ignoring
* duplicate attributes, case folding on tag names, adding missing closing tags
* based on SGML DTD information, and so on.
*
* Copyright (c) 2002 Microsoft Corporation. All rights reserved.
*
* Chris Lovett
* 
*/

using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Text;
using System.Reflection;

namespace Sgml {

    /// <summary>
    /// SGML is case insensitive, so here you can choose between converting
    /// to lower case or upper case tags.  "None" means that the case is left
    /// alone, except that end tags will be folded to match the start tags.
    /// </summary>
    public enum CaseFolding {
        None,
        ToUpper,
        ToLower
    }

    /// <summary>
    /// This stack maintains a high water mark for allocated objects so the client
    /// can reuse the objects in the stack to reduce memory allocations, this is
    /// used to maintain current state of the parser for element stack, and attributes
    /// in each element.
    /// </summary>
    internal class HWStack {
        object[] items;
        int size;
        int count;
        int growth;

        public HWStack(int growth) {
            this.growth = growth;
        }

        public int Count {
            get { return this.count; }
            set { this.count = value; }
        }
        public int Size {
            get { return this.size; }
        }
        // returns the item at the requested index or null if index is out of bounds
        public object this[int i] {
            get { return (i>=0 && i < this.size) ? items[i] : null; }
            set { this.items[i] = value; }
        }
        public object Pop(){
            this.count--;
            if (this.count>0){
                return items[this.count-1];
            }
            return null;
        }
        // This method tries to reuse a slot, if it returns null then
        // the user has to call the other Push method.
        public object Push(){
            if (this.count == this.size){
                int newsize = this.size+this.growth;
                object[] newarray = new object[newsize];
                if (this.items != null)
                    Array.Copy(this.items, newarray, this.size);
                this.size = newsize;
                this.items = newarray;
            }
            return items[this.count++];
        }        
        public void RemoveAt(int i){
            this.items[i] = null;
            Array.Copy(this.items, i+1, this.items, i, this.count - i - 1);
            this.count--;

        }
    }

    /// <summary>
    /// This class represents an attribute.  The AttDef is assigned
    /// from a validation process, and is used to provide default values.
    /// </summary>
    internal class Attribute {
        internal string Name;    // the atomized name (using XmlNameTable).
        internal AttDef DtdType; // the AttDef of the attribute from the SGML DTD.
        internal char QuoteChar; // the quote character used for the attribute value.
        internal string literalValue; // tha attribute value

        /// <summary>
        /// Attribute objects are reused during parsing to reduce memory allocations, 
        /// hence the Reset method. 
        /// </summary>
        public void Reset(string name, string value, char quote) {
            this.Name = name;
            this.literalValue = value;
            this.QuoteChar = quote;
            this.DtdType = null;
        }

        public string Value {
            get {
                if (this.literalValue != null) 
                    return this.literalValue;
                if (this.DtdType != null) 
                    return this.DtdType.Default;
                return null;
            }
            set {
                this.literalValue = value;
            }
        }

        public bool IsDefault {
            get {
                return (this.literalValue == null);
            }
        }
    }    

    /// <summary>
    /// This class models an XML node, an array of elements in scope is maintained while parsing
    /// for validation purposes, and these Node objects are reused to reduce object allocation,
    /// hence the reset method.  
    /// </summary>
    internal class Node {
        internal XmlNodeType NodeType;
        internal string Value;
        internal XmlSpace Space;
        internal string XmlLang;
        internal bool IsEmpty;        
        internal string Name;
        internal ElementDecl DtdType; // the DTD type found via validation
        internal State CurrentState;
        internal bool Simulated; // tag was injected into result stream.
        HWStack attributes = new HWStack(10);

        /// <summary>
        /// Attribute objects are reused during parsing to reduce memory allocations, 
        /// hence the Reset method. 
        /// </summary>
        public void Reset(string name, XmlNodeType nt, string value) {           
            this.Value = value;
            this.Name = name;
            this.NodeType = nt;
            this.Space = XmlSpace.None;
            this.XmlLang= null;
            this.IsEmpty = true;
            this.attributes.Count = 0;
            this.DtdType = null;
        }

        public Attribute AddAttribute(string name, string value, char quotechar, bool caseInsensitive) {
            Attribute a;
            // check for duplicates!
            for (int i = 0, n = this.attributes.Count; i < n; i++) {
                a = (Attribute)this.attributes[i];             
                if (caseInsensitive && string.Compare(a.Name, name, true) == 0) {
                    return null;
                } else if ((object)a.Name == (object)name) {
                    return null; 
                }
            }
            // This code makes use of the high water mark for attribute objects,
            // and reuses exisint Attribute objects to avoid memory allocation.
            a = (Attribute)this.attributes.Push();
            if (a == null) {
                a = new Attribute();
                this.attributes[this.attributes.Count-1] = a;
            }
            a.Reset(name, value, quotechar);
            return a;
        }

        public void RemoveAttribute(string name) {
            for (int i = 0, n = this.attributes.Count; i < n; i++) {
                Attribute a  = (Attribute)this.attributes[i];
                if (a.Name == name) {
                    this.attributes.RemoveAt(i);
                    return;
                }
            }
        }
        public void CopyAttributes(Node n) {
            for (int i = 0, len = n.attributes.Count; i < len; i++) {
                Attribute a = (Attribute)n.attributes[i];
                Attribute na = this.AddAttribute(a.Name, a.Value, a.QuoteChar, false);
                na.DtdType = a.DtdType;
            }
        }

        public int AttributeCount {
            get {
                return this.attributes.Count;
            }
        }

        public int GetAttribute(string name) {
            for (int i = 0, n = this.attributes.Count; i < n; i++) {
                Attribute a = (Attribute)this.attributes[i];
                if (a.Name == name) {
                    return i;
                }
            }
            return -1;
        }

        public Attribute GetAttribute(int i) {
            if (i>=0 && i<this.attributes.Count) {
                Attribute a = (Attribute)this.attributes[i];
                return a;
            }
            return null;
        }
    }

    // This enum is used to track the current state of te SgmlReader
    internal enum State {
        Initial,    // The initial state (Read has not been called yet)
        Markup,     // Expecting text or markup
        EndTag,     // Positioned on an end tag
        Attr,       // Positioned on an attribute
        AttrValue,  // Positioned in an attribute value
        Text,       // Positioned on a Text node.
        PartialTag, // Positioned on a text node, and we have hit a start tag
        AutoClose,  // We are auto-closing tags (this is like State.EndTag), but end tag was generated
        CData,      // We are on a CDATA type node, eg. <scipt> where we have special parsing rules.
        PartialText,
        PseudoStartTag, // we pushed a pseudo-start tag, need to continue with previous start tag.
        Eof
    }


    /// <summary>
    /// SgmlReader is an XmlReader API over any SGML document (including built in 
    /// support for HTML).  
    /// </summary>
    public class SgmlReader : XmlReader {
        SgmlDtd dtd;
        Entity current;
        State state;
        XmlNameTable nametable;
        char partial;
        object endTag;
        HWStack stack;
        Node node; // current node (except for attributes)
        // Attributes are handled separately using these members.
        Attribute a;
        int apos; // which attribute are we positioned on in the collection.
        Uri baseUri;
        StringBuilder sb;
        StringBuilder name;
        TextWriter log;
        bool foundRoot;

        // autoclose support
        Node newnode;
        int poptodepth;
        int rootCount;
        bool isHtml;
        string rootElementName;

        string href;
        string errorLogFile;
        Entity lastError;
        string proxy;
        TextReader inputStream;
        string syslit;
        string pubid;
        string subset;
        string docType;
        WhitespaceHandling whitespaceHandling;
        CaseFolding folding = CaseFolding.None;
        bool stripDocType = true;      
        string startTag = null;

        public SgmlReader() {
            Init();    
            this.nametable = new NameTable();
        }

        /// <summary>
        /// Specify the SgmlDtd object directly.  This allows you to cache the Dtd and share
        /// it across multipl SgmlReaders.  To load a DTD from a URL use the SystemLiteral property.
        /// </summary>
        public SgmlDtd Dtd {
            get { 
                LazyLoadDtd(this.baseUri);
                return this.dtd; 
            }
            set { this.dtd = value; }
        }

        private void LazyLoadDtd(Uri baseUri) {
            if (this.dtd == null) {
                if (this.syslit == null || this.syslit == "") {
                    if (this.docType != null && StringUtilities.EqualsIgnoreCase(this.docType, "html")) {
                        Assembly a = typeof(SgmlReader).Assembly;
                        string name = a.FullName.Split(',')[0]+".Html.dtd";
                        Stream stm = a.GetManifestResourceStream(name);
                        if (stm != null){
                            StreamReader sr = new StreamReader(stm);
                            this.dtd = SgmlDtd.Parse(baseUri, "HTML", null, sr, null, this.proxy, this.nametable);
                        }
                    }
                } else { 
                    if (baseUri != null) {
                        baseUri = new Uri(baseUri, this.syslit);
                    } else if (this.baseUri != null) {
                        baseUri = new Uri(this.baseUri, this.syslit);
                    } else {
                        baseUri = new Uri(new Uri(Directory.GetCurrentDirectory()+"\\"), this.syslit);
                    }
                    this.dtd = SgmlDtd.Parse(baseUri, this.docType, this.pubid, baseUri.AbsoluteUri, this.subset, this.proxy, this.nametable);
                }

                if (this.dtd != null && this.dtd.Name != null){
                    switch (this.CaseFolding){
                        case CaseFolding.ToUpper:
                            this.rootElementName = this.dtd.Name.ToUpper();
                            break;
                        case CaseFolding.ToLower:
                            this.rootElementName = this.dtd.Name.ToLower();
                            break;
                        default:
                            this.rootElementName = this.dtd.Name;
                            break;
                    }
                    this.isHtml = StringUtilities.EqualsIgnoreCase(this.dtd.Name, "html");
                }

            }
        }

        /// <summary>
        /// The name of root element specified in the DOCTYPE tag.
        /// </summary>
        public string DocType {
            get { return this.docType; }
            set { this.docType = value; }
        }

        /// <summary>
        /// The PUBLIC identifier in the DOCTYPE tag
        /// </summary>
        public string PublicIdentifier {
            get { return this.pubid; }
            set { this.pubid = value; }
        }

        /// <summary>
        /// The SYSTEM literal in the DOCTYPE tag identifying the location of the DTD.
        /// </summary>
        public string SystemLiteral {
            get { return this.syslit; }
            set { this.syslit = value; }
        }

        /// <summary>
        /// The DTD internal subset in the DOCTYPE tag
        /// </summary>
        public string InternalSubset {
            get { return this.subset; }
            set { this.subset = value; }
        }

        /// <summary>
        /// The input stream containing SGML data to parse.
        /// You must specify this property or the Href property before calling Read().
        /// </summary>
        public TextReader InputStream {
            get { return this.inputStream; }
            set { this.inputStream = value; Init();}
        }

        /// <summary>
        /// Sometimes you need to specify a proxy server in order to load data via HTTP
        /// from outside the firewall.  For example: "itgproxy:80".
        /// </summary>
        public string WebProxy {
            get { return this.proxy; }
            set { this.proxy = value; }
        }

        /// <summary>
        /// The base Uri is used to resolve relative Uri's like the SystemLiteral and
        /// Href properties.  This is a method because BaseURI is a read-only
        /// property on the base XmlReader class.
        /// </summary>
        public void SetBaseUri(string uri)  {
            this.baseUri = new Uri(uri);
        }

        /// <summary>
        /// Specify the location of the input SGML document as a URL.
        /// </summary>
        public string Href {
            get { return this.href; }
            set { this.href = value; 
                Init();
                if (this.baseUri == null) {
                    if (this.href.IndexOf("://")>0) {
                        this.baseUri = new Uri(this.href);
                    } else {
                        this.baseUri = new Uri("file:///"+Directory.GetCurrentDirectory()+"//");
                    }
                }
            }
        }

        /// <summary>
        /// Whether to strip out the DOCTYPE tag from the output (default true)
        /// </summary>
        public bool StripDocType {
            get { return this.stripDocType; }
            set { this.stripDocType = value; }
        }

        public CaseFolding CaseFolding {
            get { return this.folding; }
            set { this.folding = value; }
        }

        /// <summary>
        /// DTD validation errors are written to this stream.
        /// </summary>
        public TextWriter ErrorLog {
            get { return this.log; }
            set { this.log = value; }
        }

        public int LineNumber {
          get { return this.current.Line; }
        }
        public int LinePosition {
          get { return this.current.LinePosition; }
        }

        /// <summary>
        /// DTD validation errors are written to this log file.
        /// </summary>
        public string ErrorLogFile {
            get { return this.errorLogFile; }
            set { this.errorLogFile = value; 
                this.ErrorLog = new StreamWriter(value); }
        }

        void Log(string msg, params string[] args) {
            if (ErrorLog != null) {
                string err = String.Format(msg, args);
                if (this.lastError != this.current) {
                    err = err + "    " + this.current.Context();
                    this.lastError = this.current;
                    ErrorLog.WriteLine("### Error:"+err);
                } else {
                    string path = "";
                    if (this.current.ResolvedUri != null) {
                        path = this.current.ResolvedUri.AbsolutePath;
                    }
                    ErrorLog.WriteLine("### Error in "+
                        path+"#"+
                        this.current.Name+
                        ", line "+this.current.Line + ", position " + this.current.LinePosition + ": "+
                        err);
                }
            }
        }
        void Log(string msg, char ch) {
            Log(msg, ch.ToString());
        }


        void Init() {
            this.state = State.Initial;
            this.stack = new HWStack(10);
            this.node = Push(null, XmlNodeType.Document, null);
            this.node.IsEmpty = false;
            this.sb = new StringBuilder();
            this.name = new StringBuilder();
            this.poptodepth = 0;
            this.current = null;
            this.partial = '\0';
            this.endTag = null;
            this.a = null;
            this.apos = 0;
            this.newnode = null;
            this.rootCount = 0;
            this.foundRoot = false;
        }

        Node Push(string name, XmlNodeType nt, string value) {
            Node result = (Node)this.stack.Push();
            if (result == null) {
                result = new Node();
                this.stack[this.stack.Count-1] = result;
            }
            result.Reset(name, nt, value);
            this.node = result;
            return result;
        }

        void SwapTopNodes() {
            int top = this.stack.Count-1;
            if (top > 0) {
                Node n = (Node)this.stack[top - 1];
                this.stack[top - 1] = this.stack[top];
                this.stack[top] = n;
            }
        }

        Node Push(Node n) {
            // we have to do a deep clone of the Node object because
            // it is reused in the stack.
            Node n2 = Push(n.Name, n.NodeType, n.Value);
            n2.DtdType = n.DtdType;
            n2.IsEmpty = n.IsEmpty;
            n2.Space = n.Space;
            n2.XmlLang = n.XmlLang;
            n2.CurrentState = n.CurrentState;
            n2.CopyAttributes(n);
            this.node = n2;
            return n2;
        }

        void Pop() {
            if (this.stack.Count > 1) {
                this.node = (Node)this.stack.Pop();
            }
        }

        Node Top() {
            int top = this.stack.Count - 1;
            if (top > 0) {
                return (Node)this.stack[top];
            }
            return null;
        }

        public override XmlNodeType NodeType {
            get { 
                if (this.state == State.Attr) {
                    return XmlNodeType.Attribute;
                } 
                else if (this.state == State.AttrValue) {
                    return XmlNodeType.Text;
                }
                else if (this.state == State.EndTag || this.state == State.AutoClose) {
                    return XmlNodeType.EndElement;
                }
                return this.node.NodeType;
            }
        }

        public override string Name {
            get {
                return this.LocalName;
            }
        }

        public override string LocalName { 
            get {
                string result = null;
                if (this.state == State.Attr) {
                    result = this.a.Name;
                } 
                else if (this.state == State.AttrValue) {
                    result = null;
                }
                else {
                    result = this.node.Name;
                }

                return result;
            }
        }

        public override string NamespaceURI { 
            get {
                // SGML has no namespaces, unless this turned out to be an xmlns attribute.
                if (this.state == State.Attr && StringUtilities.EqualsIgnoreCase(this.a.Name, "xmlns")) {
                    return "http://www.w3.org/2000/xmlns/";
                }
                return String.Empty;
            }
        }

        public override string Prefix { 
            get {
                // SGML has no namespaces.
                return String.Empty;
            }
        }

        public override bool HasValue { 
            get {
                if (this.state == State.Attr || this.state == State.AttrValue) {
                    return true;
                }
                return (this.node.Value != null);
            }
        }

        public override string Value { 
            get {
                if (this.state == State.Attr || this.state == State.AttrValue) {
                    return this.a.Value;
                }
                return this.node.Value;
            }
        }

        public override int Depth { 
            get {
                if (this.state == State.Attr) {
                    return this.stack.Count;
                } 
                else if (this.state == State.AttrValue) {
                    return this.stack.Count+1;
                }
                return this.stack.Count-1;
            }
        }

        public override string BaseURI { 
            get {
                return this.baseUri == null ? "" : this.baseUri.AbsoluteUri;
            }
        }

        public override bool IsEmptyElement { 
            get {
                if (this.state == State.Markup || this.state == State.Attr || this.state == State.AttrValue) {
                    return this.node.IsEmpty;
                }
                return false;
            }
        }
        public override bool IsDefault { 
            get {
                if (this.state == State.Attr || this.state == State.AttrValue) 
                    return this.a.IsDefault;
                return false;
            }
        }
        public override char QuoteChar { 
            get {
                if (this.a != null) return this.a.QuoteChar;
                return '\0';
            }
        }

        public override XmlSpace XmlSpace { 
            get {
                for (int i = this.stack.Count-1; i > 1; i--) {
                    Node n = (Node)this.stack[i];
                    XmlSpace xs = n.Space;
                    if (xs != XmlSpace.None) return xs;
                }
                return XmlSpace.None;
            }
        }

        public override string XmlLang { 
            get {
                for (int i = this.stack.Count-1; i > 1; i--) {
                    Node n = (Node)this.stack[i];
                    string xmllang = n.XmlLang;
                    if (xmllang != null) return xmllang;
                }
                return String.Empty;
            }
        }

        public WhitespaceHandling WhitespaceHandling {
            get {
                return this.whitespaceHandling;
            } 
            set {
                this.whitespaceHandling = value;
            }
        }

        public override int AttributeCount { 
            get {
                if (this.state == State.Attr || this.state == State.AttrValue) 
                    return 0;
                if (this.node.NodeType == XmlNodeType.Element ||
                    this.node.NodeType == XmlNodeType.DocumentType)
                    return this.node.AttributeCount;
                return 0;
            }
        }

        public override string GetAttribute(string name) {
            if (this.state != State.Attr && this.state != State.AttrValue) {
                int i = this.node.GetAttribute(name);
                if (i>=0) return GetAttribute(i);
            }
            return null;
        }

        public override string GetAttribute(string name, string namespaceURI) {
            return GetAttribute(name); // SGML has no namespaces.
        }

        public override string GetAttribute(int i) {
            if (this.state != State.Attr && this.state != State.AttrValue) {
                Attribute a = this.node.GetAttribute(i);
                if (a != null)
                    return a.Value;
            }
            throw new IndexOutOfRangeException();
        }

        public override string this [ int i ] { 
            get {
                return GetAttribute(i);
            }
        }

        public override string this [ string name ] { 
            get {
                return GetAttribute(name);
            }
        }

        public override string this [ string name,string namespaceURI ] { 
            get {
                return GetAttribute(name, namespaceURI);
            }
        }

        public override bool MoveToAttribute(string name) {
            int i = this.node.GetAttribute(name);
            if (i>=0) {
                MoveToAttribute(i);
                return true;
            }
            return false;
        }

        public override bool MoveToAttribute(string name, string ns) {
            return MoveToAttribute(name);
        }

        public override void MoveToAttribute(int i) {
            Attribute a = this.node.GetAttribute(i);
            if (a != null) {
                this.apos = i;
                this.a = a;
                if (this.state != State.Attr) {
                    this.node.CurrentState = this.state;//save current state.
                }
                this.state = State.Attr;
                return;
            }
            throw new IndexOutOfRangeException();
        }

        public override bool MoveToFirstAttribute() {
            if (this.node.AttributeCount>0) {
                MoveToAttribute(0);
                return true;
            }
            return false;
        }

        public override bool MoveToNextAttribute() {
            if (this.state != State.Attr && this.state != State.AttrValue) {
                return MoveToFirstAttribute();
            }
            if (this.apos<this.node.AttributeCount-1) {
                MoveToAttribute(this.apos+1);
                return true;
            }
            return false;
        }

        public override bool MoveToElement() {
            if (this.state == State.Attr || this.state == State.AttrValue) {
                this.state = this.node.CurrentState;
                this.a = null;
                return true;
            }
            return (this.node.NodeType == XmlNodeType.Element);
        }

        bool IsHtml {
            get {
              return this.isHtml;
            }
        }

        public Encoding GetEncoding(){
            if (this.current == null) {
                OpenInput();
            }
            return this.current.GetEncoding();
        }

        void OpenInput(){
            LazyLoadDtd(this.baseUri);

            if (this.Href != null) {
                this.current = new Entity("#document", null, this.href, this.proxy);
            } else if (this.inputStream != null) {
                this.current = new Entity("#document", null, this.inputStream, this.proxy);           
            } else {
                throw new InvalidOperationException("You must specify input either via Href or InputStream properties");
            }
            this.current.Html = this.IsHtml;
            this.current.Open(null, this.baseUri);
            if (this.current.ResolvedUri != null)
                this.baseUri = this.current.ResolvedUri;

            if (this.current.Html && this.dtd == null){
                this.docType = "HTML";
                LazyLoadDtd(this.baseUri);
            }
        }

        public override bool Read() {
            if (current == null) {
                OpenInput();
            }
            State start = this.state;
            if (node.Simulated) {
                // return the next node
                node.Simulated = false;
                this.node = Top();
                this.state = this.node.CurrentState;
                return true;
            }

            bool foundnode = false;
            while (! foundnode) {
                switch (this.state) {
                    case State.Initial:
                        this.state = State.Markup;
                        this.current.ReadChar();
                        goto case State.Markup;
                    case State.Eof:
                        if (this.current.Parent != null) {
                            this.current.Close();
                            this.current = this.current.Parent;
                        } else {                           
                            return false;
                        }
                        break;
                    case State.EndTag:
                        if (this.endTag == (object)this.node.Name) {
                            Pop(); // we're done!
                            this.state = State.Markup;
                            goto case State.Markup;
                        }                     
                        Pop(); // close one element
                        foundnode = true;// return another end element.
                        break;
                    case State.Markup:
                        if (this.node.IsEmpty) {
                            Pop();
                        }
                        Node n = this.node;
                        foundnode = ParseMarkup();
                        break;
                    case State.PartialTag:
                        Pop(); // remove text node.
                        this.state = State.Markup;
                        foundnode = ParseTag(this.partial);
                        break;
                    case State.PseudoStartTag:
                        foundnode = ParseStartTag('<');                        
                        break;
                    case State.AutoClose:
                        Pop(); // close next node.
                        if (this.stack.Count <= this.poptodepth) {
                            this.state = State.Markup;
                            if (this.newnode != null) {
                                Push(this.newnode); // now we're ready to start the new node.
                                this.newnode = null;
                                this.state = State.Markup;
                            } else if (this.node.NodeType == XmlNodeType.Document) {
                                this.state = State.Eof;
                                goto case State.Eof;
                            }
                        } 
                        foundnode = true;
                        break;
                    case State.CData:
                        foundnode = ParseCData();
                        break;
                    case State.Attr:
                        goto case State.AttrValue;
                    case State.AttrValue:
                        this.state = State.Markup;
                        goto case State.Markup;
                    case State.Text:
                        Pop();
                        goto case State.Markup;
                    case State.PartialText:
                        if (ParseText(this.current.Lastchar, false)) {
                            this.node.NodeType = XmlNodeType.Whitespace;
                        }
                        foundnode = true;
                        break;
                }
                if (foundnode && this.node.NodeType == XmlNodeType.Whitespace && this.whitespaceHandling == WhitespaceHandling.None) {
                    // strip out whitespace (caller is probably pretty printing the XML).
                    foundnode = false;
                }
                if (!foundnode && this.state == State.Eof && this.stack.Count>1) {
                    this.poptodepth = 1;
                    state = State.AutoClose;
                    this.node = Top();
                    return true;
                }
            }
            if (!foundRoot && (this.NodeType == XmlNodeType.Element ||
                    this.NodeType == XmlNodeType.Text ||
                    this.NodeType == XmlNodeType.CDATA)) {
                foundRoot = true;
                if (this.IsHtml && (this.NodeType != XmlNodeType.Element ||
                    string.Compare(this.LocalName, "html", true, System.Globalization.CultureInfo.InvariantCulture) != 0)) {
                    // Simulate an HTML root element!
                    this.node.CurrentState = this.state;
                    Node root = Push("html", XmlNodeType.Element, null);
                    SwapTopNodes(); // make html the outer element.
                    this.node = root;
                    root.Simulated = true;
                    root.IsEmpty = false;
                    this.state = State.Markup;
                    //this.state = State.PseudoStartTag;
                    //this.startTag = name;
                }
                return true;
            }
            return true;
        }

        bool ParseMarkup() {
            char ch = this.current.Lastchar;
            if (ch == '<') {
                ch = this.current.ReadChar();
                return ParseTag(ch);
            } 
            else if (ch != Entity.EOF) {
                if (this.node.DtdType != null && this.node.DtdType.ContentModel.DeclaredContent == DeclaredContent.CDATA) {
                    // e.g. SCRIPT or STYLE tags which contain unparsed character data.
                    this.partial = '\0';
                    this.state = State.CData;
                    return false;
                }
                else if (ParseText(ch, true)) {
                    this.node.NodeType = XmlNodeType.Whitespace;
                }
                return true;
            }
            this.state = State.Eof;
            return false;
        }

        static string declterm = " \t\r\n><";
        bool ParseTag(char ch) {
            if (ch == '%') {
                return ParseAspNet();
            }
            if (ch == '!') {
                ch = this.current.ReadChar();
                if (ch == '-') {
                    return ParseComment();
                } else if (ch == '[') {
                    return ParseConditionalBlock();
                }else if (ch != '_' && !Char.IsLetter(ch)) {
                    // perhaps it's one of those nasty office document hacks like '<![if ! ie ]>'
                    string value = this.current.ScanToEnd(this.sb, "Recovering", ">"); // skip it
                    Log("Ignoring invalid markup '<!"+value+">");
                    return false;
                }
                else {
                    string name = this.current.ScanToken(this.sb, SgmlReader.declterm, false);
                    if (name == "DOCTYPE") {
                        ParseDocType();
                        // In SGML DOCTYPE SYSTEM attribute is optional, but in XML it is required,
                        // therefore if there is no SYSTEM literal then add an empty one.
                        if (this.GetAttribute("SYSTEM") == null && this.GetAttribute("PUBLIC") != null) {
                            this.node.AddAttribute("SYSTEM", "", '"', this.folding == CaseFolding.None);
                        }
                        if (stripDocType) {
                            return false;
                        } else {
                            this.node.NodeType = XmlNodeType.DocumentType;
                            return true;
                        }
                    } 
                    else {
                        Log("Invalid declaration '<!{0}...'.  Expecting '<!DOCTYPE' only.", name);
                        this.current.ScanToEnd(null, "Recovering", ">"); // skip it
                        return false;
                    }
                }
            } 
            else if (ch == '?') {
                this.current.ReadChar();// consume the '?' character.
                return ParsePI();
            }
            else if (ch == '/') {
                return ParseEndTag();
            }
            else {
                return ParseStartTag(ch);
            }
       }

        string ScanName(string terminators) {
            string name = this.current.ScanToken(this.sb, terminators, false);
            switch (this.folding){
                case CaseFolding.ToUpper:
                    name = name.ToUpper();
                    break;
                case CaseFolding.ToLower:
                    name = name.ToLower();
                    break;
            }
            return this.nametable.Add(name);
        }

        static string tagterm = " \t\r\n=/><";
        static string aterm = " \t\r\n='\"/>";
        static string avterm = " \t\r\n>";
        bool ParseStartTag(char ch) {
            string name = null;
            if (state != State.PseudoStartTag){
                if (SgmlReader.tagterm.IndexOf(ch)>=0) {
                    this.sb.Length = 0;
                    this.sb.Append('<');
                    this.state = State.PartialText;
                    return false;
                }
                name = ScanName(SgmlReader.tagterm);                
            } else {
                name = this.startTag;
                state = State.Markup;
            }
            Node n = Push(name, XmlNodeType.Element, null);
            n.IsEmpty = false;
            Validate(n);
            ch = this.current.SkipWhitespace();
            while (ch != Entity.EOF && ch != '>') {
                if (ch == '/') {
                    n.IsEmpty = true;
                    ch = this.current.ReadChar();
                    if (ch != '>') {
                        Log("Expected empty start tag '/>' sequence instead of '{0}'", ch);
                        this.current.ScanToEnd(null, "Recovering", ">");
                        return false;
                    }
                    break;
                } 
                else if (ch == '<') {
                    Log("Start tag '{0}' is missing '>'", name);
                    break;
                }
                string aname = ScanName(SgmlReader.aterm);
                ch = this.current.SkipWhitespace();
                if (aname == "," || aname == "=" || aname == ":" || aname == ";") {
                    continue;
                }
                string value = null;
                char quote = '\0';
                if (ch == '=' || ch == '"' || ch == '\'') {
                    if (ch == '=' ){
                        this.current.ReadChar();
                        ch = this.current.SkipWhitespace();
                    }
                    if (ch == '\'' || ch == '\"') {
                        quote = ch;
                        value = ScanLiteral(this.sb, ch);
                    } 
                    else if (ch != '>') {
                        string term = SgmlReader.avterm;
                        value = this.current.ScanToken(this.sb, term, false);
                    }
                } 
                if (aname.Length > 0) {
                    Attribute a = n.AddAttribute(aname, value, quote, this.folding == CaseFolding.None);
                    if (a == null) {
                        Log("Duplicate attribute '{0}' ignored", aname);
                    } else {
                        ValidateAttribute(n, a);
                    }
                }
                ch = this.current.SkipWhitespace();
            }
            if (ch == Entity.EOF) {
                this.current.Error("Unexpected EOF parsing start tag '{0}'", name);
            } 
            else if (ch == '>') {
                this.current.ReadChar(); // consume '>'
            }
            if (this.Depth == 1) {
                if (this.rootCount == 1) {
                    // Hmmm, we found another root level tag, soooo, the only
                    // thing we can do to keep this a valid XML document is stop
                    this.state = State.Eof;
                    return false;
                }
                this.rootCount++;
            }
            ValidateContent(n);
            return true;
        }

        bool ParseEndTag() {
            this.state = State.EndTag;
            this.current.ReadChar(); // consume '/' char.
            string name = this.ScanName(SgmlReader.tagterm);
            char ch = this.current.SkipWhitespace();
            if (ch != '>') {
                Log("Expected empty start tag '/>' sequence instead of '{0}'", ch);
                this.current.ScanToEnd(null, "Recovering", ">");
            }
            this.current.ReadChar(); // consume '>'

            this.endTag = name;
            // Make sure there's a matching start tag for it.                        
            bool caseInsensitive = (this.folding == CaseFolding.None);
            this.node = (Node)this.stack[this.stack.Count-1];
            for (int i = this.stack.Count-1; i>0; i--) {
                Node n = (Node)this.stack[i];
                if (caseInsensitive && string.Compare(n.Name, name, true) == 0) {
                    this.endTag = n.Name;
                    return true;
                } else if ((object)n.Name == (object)name) {
                    return true;
                }
            }
            Log("No matching start tag for '</{0}>'", name);
            this.state = State.Markup;
            return false;
        }

        bool ParseAspNet() {
            string value = "<%" + this.current.ScanToEnd(this.sb, "AspNet", "%>") + "%>";
            Push(null, XmlNodeType.CDATA, value);         
            return true;
        }

        bool ParseComment() {
            char ch = this.current.ReadChar();
            if (ch != '-') {
                Log("Expecting comment '<!--' but found {0}", ch);
                this.current.ScanToEnd(null, "Comment", ">");
                return false;
            }
            string value = this.current.ScanToEnd(this.sb, "Comment", "-->");
            
            // Make sure it's a valid comment!
            int i = value.IndexOf("--");
            while (i>=0) {
                int j = i+2;
                while (j<value.Length && value[j]=='-')
                    j++;
                if (i>0) {
                    value = value.Substring(0, i-1)+"-"+value.Substring(j);
                } 
                else {
                    value = "-"+value.Substring(j);
                }
                i = value.IndexOf("--");
            }
            if (value.Length>0 && value[value.Length-1] == '-') {
                value += " "; // '-' cannot be last character
            }
            Push(null, XmlNodeType.Comment, value);         
            return true;
        }

        static string cdataterm = "\t\r\n[<>";
        bool ParseConditionalBlock(){
            char ch = current.ReadChar(); // skip '['
            ch = current.SkipWhitespace();
            string name = current.ScanToken(sb, cdataterm, false);
            if (name != "CDATA"){
                Log("Expecting CDATA but found '{0}'", name);
                current.ScanToEnd(null, "CDATA", ">");
                return false;
            }
            ch = current.SkipWhitespace();
            if (ch != '[') {
                Log("Expecting '[' but found '{0}'", ch);
                current.ScanToEnd(null, "CDATA", ">");
                return false;
            }
            string value = current.ScanToEnd(sb, "CDATA", "]]>");
                        
            Push(null, XmlNodeType.CDATA, value);         
            return true;
        }

        static string dtterm = " \t\r\n>";
        void ParseDocType() {
            char ch = this.current.SkipWhitespace();
            string name = this.ScanName(SgmlReader.dtterm);
            Push(name, XmlNodeType.DocumentType, null);
            ch = this.current.SkipWhitespace();
            if (ch != '>') {
                string subset = "";
                string pubid = "";
                string syslit = "";

                if (ch != '[') {
                    string token = this.current.ScanToken(this.sb, SgmlReader.dtterm, false);
                    if (token == "PUBLIC") {
                        ch = this.current.SkipWhitespace();
                        if (ch == '\"' || ch == '\'') {
                            pubid = this.current.ScanLiteral(this.sb, ch);
                            this.node.AddAttribute(token, pubid, ch, this.folding == CaseFolding.None);  
                        }
                    } 
                    else if (token != "SYSTEM") {
                        Log("Unexpected token in DOCTYPE '{0}'", token);
                        this.current.ScanToEnd(null, "DOCTYPE", ">");
                    }
                    ch = this.current.SkipWhitespace();
                    if (ch == '\"' || ch == '\'') {
                        token = this.nametable.Add("SYSTEM");
                        syslit = this.current.ScanLiteral(this.sb, ch);
                        this.node.AddAttribute(token, syslit, ch, this.folding == CaseFolding.None);  
                    }
                    ch = this.current.SkipWhitespace();
                }
                if (ch == '[') {
                    subset = this.current.ScanToEnd(this.sb, "Internal Subset", "]");
                    this.node.Value = subset;
                }
                ch = this.current.SkipWhitespace();
                if (ch != '>') {
                    Log("Expecting end of DOCTYPE tag, but found '{0}'", ch);
                    this.current.ScanToEnd(null, "DOCTYPE", ">");
                }

                if (this.dtd == null) {
                    this.docType = name;
                    this.pubid = pubid;
                    this.syslit = syslit;
                    this.subset = subset;
                    LazyLoadDtd(this.current.ResolvedUri);
                }
            }           
            this.current.ReadChar();
        }

        static string piterm = " \t\r\n?";
        bool ParsePI() {
            string name = this.current.ScanToken(this.sb, SgmlReader.piterm, false);
            string value = null;
            if (this.current.Lastchar != '?') {
                // Notice this is not "?>".  This is because Office generates bogus PI's that end with "/>".
                value = this.current.ScanToEnd(this.sb, "Processing Instruction", ">");
            }
            else {
                // error recovery.
                value = this.current.ScanToEnd(this.sb, "Processing Instruction", ">");
            }
            // skip xml declarations, since these are generated in the output instead.
            if (name != "xml"){
                Push(nametable.Add(name), XmlNodeType.ProcessingInstruction, value);
                return true;
            }
            return false;
        }

        bool ParseText(char ch, bool newtext) {
            bool ws = !newtext || this.current.IsWhitespace;
            if (newtext) this.sb.Length = 0;
            //this.sb.Append(ch);
            //ch = this.current.ReadChar();
            this.state = State.Text;
            while (ch != Entity.EOF) {
                if (ch == '<') {
                    ch = this.current.ReadChar();
                    if (ch == '/' || ch == '!' || ch == '?' || Char.IsLetter(ch)) {
                        // Hit a tag, so return XmlNodeType.Text token
                        // and remember we partially started a new tag.
                        this.state = State.PartialTag;
                        this.partial = ch;
                        break;
                    } 
                    else {
                        // not a tag, so just proceed.
                        this.sb.Append('<'); 
                        this.sb.Append(ch);
                        ws = false;
                        ch = this.current.ReadChar();
                    }
                } 
                else if (ch == '&') {
                    ExpandEntity(this.sb, '<');
                    ws = false;
                    ch = this.current.Lastchar;
                }
                else {
                    if (!this.current.IsWhitespace) ws = false;
                    this.sb.Append(ch);
                    ch = this.current.ReadChar();
                }
            }
            string value = this.sb.ToString();
            Push(null, XmlNodeType.Text, value);
            return ws;
        }

        // This version is slightly different from Entity.ScanLiteral in that
        // it also expands entities.
        public string ScanLiteral(StringBuilder sb, char quote) {
            sb.Length = 0;
            char ch = this.current.ReadChar();
            while (ch != Entity.EOF && ch != quote ) {
                if (ch == '&') {
                    ExpandEntity(this.sb, quote);
                    ch = this.current.Lastchar;
                }               
                else {
                    sb.Append(ch);
                    ch = this.current.ReadChar();
                }
            }
            this.current.ReadChar(); // consume end quote.          
            return sb.ToString();
        }

        bool ParseCData() {
            // Like ParseText(), only it doesn't allow elements in the content.  
            // It allows comments and processing instructions and text only and
            // text is not returned as text but CDATA (since it may contain angle brackets).
            // And initial whitespace is ignored.  It terminates when we hit the
            // end tag for the current CDATA node (e.g. </style>).
            bool ws = this.current.IsWhitespace;
            this.sb.Length = 0;
            char ch = this.current.Lastchar;
            if (this.partial != '\0') {
                Pop(); // pop the CDATA
                switch (this.partial) {
                    case '!':
                        this.partial = ' '; // and pop the comment next time around
                        return ParseComment();
                    case '?':
                        this.partial = ' '; // and pop the PI next time around
                        return ParsePI();
                    case '/':
                        this.state = State.EndTag;
                        return true;    // we are done!
                    case ' ':
                        break; // means we just needed to pop the Comment, PI or CDATA.
                }
            } else {
                ch = this.current.ReadChar();
            }            
            
            // if this.partial == '!' then parse the comment and return
            // if this.partial == '?' then parse the processing instruction and return.            
            while (ch != Entity.EOF) {
                if (ch == '<') {
                    ch = this.current.ReadChar();
                    if (ch == '!') {
                        ch = this.current.ReadChar();
                        if (ch == '-') {
                            // return what CDATA we have accumulated so far
                            // then parse the comment and return to here.
                            if (ws) {
                                this.partial = ' '; // pop comment next time through
                                return ParseComment();
                            } 
                            else {
                                // return what we've accumulated so far then come
                                // back in and parse the comment.
                                this.partial = '!';
                                break; 
                            }
#if FIX
                        } else if (ch == '['){
                            // We are about to wrap this node as a CDATA block because of it's
                            // type in the DTD, but since we found a CDATA block in the input
                            // we have to parse it as a CDATA block, otherwise we will attempt
                            // to output nested CDATA blocks which of course is illegal.
                            if (this.ParseConditionalBlock()){
                                this.partial = ' ';
                                return true;
                            }
#endif
                        } else {
                            // not a comment, so ignore it and continue on.
                            this.sb.Append('<');
                            this.sb.Append('!');
                            this.sb.Append(ch);
                            ws = false;
                        }
                    } 
                    else if (ch == '?') {
                        // processing instruction.
                        this.current.ReadChar();// consume the '?' character.
                        if (ws) {
                            this.partial = ' '; // pop PI next time through
                            return ParsePI();
                        } 
                        else {
                            this.partial = '?';
                            break;
                        }
                    }
                    else if (ch == '/') {
                        // see if this is the end tag for this CDATA node.
                        string temp = this.sb.ToString();
                        if (ParseEndTag() && this.endTag == (object)this.node.Name) {
                            if (ws || temp == "") {
                                // we are done!
                                return true;
                            } 
                            else {
                                // return CDATA text then the end tag
                                this.partial = '/';
                                this.sb.Length = 0; // restore buffer!
                                this.sb.Append(temp); 
                                this.state = State.CData;
                                break;
                            }
                        } 
                        else {
                            // wrong end tag, so continue on.
                            this.sb.Length = 0; // restore buffer!
                            this.sb.Append(temp); 
                            this.sb.Append("</"+this.endTag+">");
                            ws = false;
                        }
                    }
                    else {
                        // must be just part of the CDATA block, so proceed.
                        this.sb.Append('<'); 
                        this.sb.Append(ch);
                        ws = false;
                    }
                } 
                else {
                    if (!this.current.IsWhitespace && ws) ws = false;
                    this.sb.Append(ch);
                }
                ch = this.current.ReadChar();
            }
            string value = this.sb.ToString();
            Push(null, XmlNodeType.CDATA, value);
            if (this.partial == '\0')
                this.partial = ' ';// force it to pop this CDATA next time in.
            return true;
        }

        void ExpandEntity(StringBuilder sb, char terminator) {
            char ch = this.current.ReadChar();
            if (ch == '#') {
                string charent = this.current.ExpandCharEntity();
                sb.Append(charent);
                ch = this.current.Lastchar;
            } 
            else {
                this.name.Length = 0;
                while (ch != Entity.EOF && 
                    (Char.IsLetter(ch) || ch == '_' || ch == '-')) {
                    this.name.Append(ch);
                    ch = this.current.ReadChar();
                }
                string name = this.name.ToString();
                if (this.dtd != null && name != "") {
                    Entity e = (Entity)this.dtd.FindEntity(name);
                    if (e != null) {
                        if (e.Internal) {
                            sb.Append(e.Literal);
                            if (ch != terminator) 
                                ch = this.current.ReadChar();
                            return;
                        } 
                        else {
                            Entity ex = new Entity(name, e.PublicId, e.Uri, this.current.Proxy);
                            e.Open(this.current, new Uri(e.Uri));
                            this.current = ex;
                            this.current.ReadChar();
                            return;
                        }
                    } 
                    else {
                        Log("Undefined entity '{0}'", name);
                    }
                }
                // Entity is not defined, so just keep it in with the rest of the
                // text.
                sb.Append("&");
                sb.Append(name);
                if (ch != terminator) {
                    sb.Append(ch);
                    ch = this.current.ReadChar();
                }
            }
        }

        public override bool EOF { 
            get {
                return this.state == State.Eof;
            }
        }

        public override void Close() {
            if (this.current != null) {
                this.current.Close();
                this.current = null;
            }
            if (this.log != null) {
                this.log.Close();
                this.log = null;
            }
        }

        public override ReadState ReadState { 
            get {
                if (this.state == State.Initial) return ReadState.Initial;
                else if (this.state == State.Eof) return ReadState.EndOfFile;
                return ReadState.Interactive;
            }
        }

        public override string ReadString() {
            if (this.node.NodeType == XmlNodeType.Element) {
                this.sb.Length = 0;
                while (Read()) {
                    switch (this.NodeType) {
                        case XmlNodeType.CDATA:
                        case XmlNodeType.SignificantWhitespace:
                        case XmlNodeType.Whitespace:
                        case XmlNodeType.Text:
                            this.sb.Append(this.node.Value);
                            break;
                        default:
                            return this.sb.ToString();
                    }
                }
                return this.sb.ToString();
            }
            return this.node.Value;
        }


        public override string ReadInnerXml() {
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xw.Formatting = Formatting.Indented;
            switch (this.NodeType) {
                case XmlNodeType.Element:
                    Read();
                    while (!this.EOF && this.NodeType != XmlNodeType.EndElement) {
                        xw.WriteNode(this, true);
                    }
                    Read(); // consume the end tag
                    break;
                case XmlNodeType.Attribute:
                    sw.Write(this.Value);
                    break;
                default:
                    // return empty string according to XmlReader spec.
                    break;
            }
            xw.Close();
            return sw.ToString();
        }

        public override string ReadOuterXml() {
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xw.Formatting = Formatting.Indented;
            xw.WriteNode(this, true);
            xw.Close();
            return sw.ToString();
        }

        public override XmlNameTable NameTable { 
            get {
                return this.nametable;
            }
        }

        public override string LookupNamespace(string prefix) {           
            return null;// there are no namespaces in SGML.
        }

        public override void ResolveEntity() {
            // We never return any entity reference nodes, so this should never be called.
            throw new InvalidOperationException("Not on an entity reference.");
        }

        public override bool ReadAttributeValue() {
            if (this.state == State.Attr) {
                this.state = State.AttrValue;
                return true;
            } 
            else if (this.state == State.AttrValue) {
                return false;
            }
            throw new InvalidOperationException("Not on an attribute.");
        }   

        void Validate(Node node) {
            if (this.dtd != null) {
                ElementDecl e = this.dtd.FindElement(node.Name);
                if (e != null) {
                    node.DtdType = e;
                    if (e.ContentModel.DeclaredContent == DeclaredContent.EMPTY) 
                        node.IsEmpty = true;
                }
            }
        }

        void ValidateAttribute(Node node, Attribute a) {
            ElementDecl e = node.DtdType;
            if (e != null) {
                AttDef ad = e.FindAttribute(a.Name);
                if (ad != null) {
                    a.DtdType = ad;
                }
            }
        }   

        void ValidateContent(Node node) {
            if (this.dtd != null) {
                // See if this element is allowed inside the current element.
                // If it isn't, then auto-close elements until we find one
                // that it is allowed to be in.                                  
                string name = this.nametable.Add(node.Name.ToUpper()); // DTD is in upper case
                int i = 0;
                int top = this.stack.Count-2;
                if (node.DtdType != null) { 
                    // it is a known element, let's see if it's allowed in the
                    // current context.
                    for (i = top; i>0; i--) {
                        Node n = (Node)this.stack[i];
                        if (n.IsEmpty) 
                            continue; // we'll have to pop this one
                        ElementDecl f = n.DtdType;
                        if (f != null) {
                            if (f.Name == this.dtd.Name)
                                break; // can't pop the root element.
                            if (f.CanContain(name, this.dtd)) {
                                break;
                            } 
                            else if (!f.EndTagOptional) {
                                // If the end tag is not optional then we can't
                                // auto-close it.  We'll just have to live with the
                                // junk we've found and move on.
                                break;
                            }
                        } 
                        else {
                            // Since we don't understand this tag anyway,
                            // we might as well allow this content!
                            break;
                        }
                    }
                }
                if (i == 0) {
                    // Tag was not found or is not allowed anywhere, ignore it and 
                    // continue on.
                }
                else if (i < top) {
                    Node n = (Node)this.stack[top];
                    if (i == top - 1 && name == n.Name) {
                        // e.g. p not allowed inside p, not an interesting error.
                    } else {
                        string closing = "";
                        for (int k = top; k >= i+1; k--) {
                            if (closing != "") closing += ",";
                            Node n2 = (Node)this.stack[k];
                            closing += "<"+n2.Name+">";
                        }
                        Log("Element '{0}' not allowed inside '{1}', closing {2}.", 
                            name, n.Name, closing);
                    }
                    this.state = State.AutoClose;
                    this.newnode = node;
                    Pop(); // save this new node until we pop the others
                    this.poptodepth = i+1;
                }
            }
        }
    }
}