using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;
using Trinity.Diagnostics;

namespace Trinity.Configuration
{
    class XmlConfiguration
    {
        #region public Fields
        HashSet<string> m_importedFiles = new HashSet<string>();
        Dictionary<string, List<XElement>> m_loadedTemplates = new Dictionary<string, List<XElement>>();
        ParseUnit m_finalUnit;
        #endregion
      
        #region Properties
        /// <summary>
        /// Gets path of the Configuration file
        /// </summary>
        public string RootConfigFullPath
        {
            get;
            set;
        }
        /// <summary>
        /// Gets version of the current Configuration file to ensure compatibility
        /// </summary>
        public string RootConfigVersion
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the cluster configuration section
        /// </summary>
        public IEnumerable<XElement> ClusterSections
        {
            get { return m_finalUnit.Elements.Where(_ => _.Name == ConfigurationConstants.Tags.CLUSTER); }
        }
        /// <summary>
        /// Gets the local configuration section
        /// </summary>
        public XElement LocalSection
        {
            get { return m_finalUnit.Elements.FirstOrDefault(_ => _.Name == ConfigurationConstants.Tags.LOCAL) ?? new XElement(ConfigurationConstants.Tags.LOCAL); }
        }
        #endregion
      
        #region Data Structure
        /// <summary>
        /// Structure definition of a configuration file
        /// </summary>
        private class ParseUnit
        {
            public string FullPath;
            public string Version;
            public List<XElement> Elements;
            public ParseUnit(IEnumerable<XElement> elements = null, string path = null, string version = null)
            {
                FullPath = path;
                Version = version;
                Elements = (elements == null ? new List<XElement>() : elements.ToList());
            }
        }
        #endregion
      
        #region Constructor
        /// <summary>
        /// Parse a configuration file with the specified file name.
        /// </summary>
        /// <param name="filename"></param>
        private XmlConfiguration(string filename)
        {
            RootConfigFullPath = GetFullPath(filename);
            var loaded = LoadFile(filename);
            //filename shoule be added in case filename has the same directory with a directory in a import entry
            m_importedFiles.Add(RootConfigFullPath);

            RootConfigVersion = loaded.Version;
            m_finalUnit = Pipeline(loaded);
        }
        /// <summary>
        /// Load configuration file with the specified file name.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        static public XmlConfiguration Load(string fileName)
        {
            return new XmlConfiguration(fileName);
        }
        #endregion
       
        #region Processing Pipeline
        /// <summary>
        /// In a pipelined manner manipulate configuration file and the order is import node, template node.
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        ParseUnit Pipeline(ParseUnit elements)
        {
            var importProcessed = ProcessImports(elements);
            var templateProcessed = ProcessTemplates(importProcessed);
            var mergeProcessed = ProcessMerge(templateProcessed);
            return mergeProcessed;
        }
        /// <summary>
        /// Merge the multiple local configuration sections and multiple cluster configuration sections
        /// </summary>
        /// <param name="inputUnit"></param>
        /// <returns></returns>
        private ParseUnit ProcessMerge(ParseUnit inputUnit)
        {
            ParseUnit mergedSections = new ParseUnit();


            // merge local
            var localSections = inputUnit.Elements
                .Where(_ => _.Name == ConfigurationConstants.Tags.LOCAL);
            var mergedEntries = MergeSections(new List<IEnumerable<XElement>>(localSections.Select(_ => _.Elements())));
            XElement localSection = new XElement(ConfigurationConstants.Tags.LOCAL);
            foreach (var entry in mergedEntries)
                localSection.Add(entry);
            mergedSections.Elements.Add(localSection);

            var clusterSections = inputUnit.Elements
                .Where(_ => _.Name == ConfigurationConstants.Tags.CLUSTER)
                .GroupBy(_ =>
                {
                    var attr = _.Attribute(ConfigurationConstants.Attrs.ID);
                    if (attr != null) { return attr.Value; }
                    else { return null; }
                })
                .Select(_ =>
            {
                foreach (var cluster in _)
                {
                    foreach (var node in cluster.Elements())
                    {
                        var entries = MergeSections(new List<IEnumerable<XElement>> { node.Elements() });
                        node.RemoveNodes();
                        foreach (var entry in entries)
                            node.Add(entry);
                    }
                    var defaultNodes = cluster.Elements().Where(__ => __.Name == ConfigurationConstants.Tags.DEFAULT);
                    Dictionary<string, XElement> defaultEntries = new Dictionary<string, XElement>();

                    foreach (var node in defaultNodes)
                        foreach (var entry in node.Elements())
                            defaultEntries[entry.Name.LocalName] = entry;

                    foreach (var node in cluster.Elements())
                        foreach (var entry in defaultEntries)
                            if (node.Element(entry.Key) == null)
                                node.Add(entry.Value);
                }
                return _;
            }).Select(_ =>
            {
                return MergeSections(new List<IEnumerable<XElement>> { _ }).First();
            });
            mergedSections.Elements.AddRange(clusterSections);

            return mergedSections;
        }
        #endregion

        #region Helper Functions
        /// <summary>
        /// Gets the absolute path for the specified file name.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string GetFullPath(string filename)
        {
            return Path.GetFullPath(filename);
        }

        /// <summary>
        /// Load configuration file with specified file name by linq
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        ParseUnit LoadFile(string filename)
        {
            var document = XDocument.Load(filename);
            var rootNode = document.Root;

            if(rootNode.GetDefaultNamespace() == string.Empty && rootNode.Name.NamespaceName == string.Empty)
            {
                rootNode = SetDefaultNamespace(rootNode);
            }

            if (rootNode == null || ConfigurationConstants.Tags.ROOT_NODE != rootNode.Name)
            {
                Log.WriteLine(LogLevel.Error, "File format not recognized.");
                throw new FormatException("File format not recognized.");
            }
            var versionAttr = rootNode.Attribute(ConfigurationConstants.Attrs.CONFIG_VERSION);
            var version = versionAttr == null ? ConfigurationConstants.Values.LEGACYVER : versionAttr.Value;
            return new ParseUnit(rootNode.Elements(), GetFullPath(filename), version);
        }

        private XElement SetDefaultNamespace(XElement element)
        {
            XName name = element.Name.NamespaceName == string.Empty?
                ConfigurationConstants.NS + element.Name.LocalName :
                element.Name;
            return new XElement(name, element.Elements().Select(SetDefaultNamespace));
        }

        /// <summary>
        /// Each section is represented by <![CDATA[IEnumerable<XElement>]]>, obtained by calling XElement.Elements()
        /// Later XElements with the same Name will overwrite earlier entries
        /// This routine can be used for:
        /// 1. Merging configuration entries in a configuration section.
        /// 2. Merging local configuration sections.
        /// 3. Merging server/proxy sections in a cluster node.
        /// </summary>
        /// <param name="sections"></param>
        /// <returns></returns>
        IEnumerable<XElement> MergeSections(IEnumerable<IEnumerable<XElement>> sections)
        {
            // remove duplicates while preserving the order
            var list = new List<XElement>();
            var indexMap = new Dictionary<string, int>();
            foreach (var section in sections)
            {
                foreach (var element in section)
                {
                    string key = element.Name.ToString();
                    if (indexMap.ContainsKey(key))
                    {
                        list[indexMap[key]] = element;
                    }
                    else
                    {
                        indexMap[key] = list.Count;
                        list.Add(element);
                    }
                }
            }
            return list;
        }

        KeyValuePair<K, V> CreatePair<K, V>(K key, V value)
        {
            return new KeyValuePair<K, V>(key, value);
        }
        #endregion

        #region Processing Imports
        ParseUnit ProcessImports(ParseUnit parentUnit)
        {
            ParseUnit result = new ParseUnit();
            foreach (var e in parentUnit.Elements)
            {
                if (e.Name == ConfigurationConstants.Tags.IMPORT)
                    result.Elements.AddRange(ProcessImportNode(e, parentUnit).Elements);
                else
                    result.Elements.Add(e);
            }
            return result;
        }

        ParseUnit ImportFile(string filename, ParseUnit parentUnit)
        {
            var fullpath = GetFullPath(filename);
            if (m_importedFiles.Contains(fullpath))
            {
                Log.WriteLine(LogLevel.Warning, "Ignoring file '" + fullpath + "' because it is already imported.");
                return new ParseUnit();
            }

            var loaded = LoadFile(filename);
            if (loaded.Version != RootConfigVersion)
            {
                Log.WriteLine(LogLevel.Warning, "Ignoring file '" + filename + "' because of confliting config version:");
                Log.WriteLine(LogLevel.Warning, "Expected:" + RootConfigVersion + ", Found:" + loaded.Version + "");
                return new ParseUnit();
            }
            Log.WriteLine(LogLevel.Info, "TrinityConfig:" + fullpath + " imported.");
            m_importedFiles.Add(fullpath);
            return loaded;
        }

        ParseUnit ProcessImportNode(XElement importNode, ParseUnit parentUnit)
        {
            var fileAttr = importNode.Attribute(ConfigurationConstants.Attrs.FILE);
            var dirAttr = importNode.Attribute(ConfigurationConstants.Attrs.DIRECTORY);

            if (fileAttr != null)
            {
                var path = fileAttr.Value;
                if (!File.Exists(path))
                {
                    Log.WriteLine(LogLevel.Info, "Path " + path + " does not exist or is not a file.");
                    throw new FileNotFoundException("Path " + path + " does not exist or is not a file.");
                }
                var loadedImportFile = ImportFile(path, parentUnit);
                return Pipeline(loadedImportFile);
            }

            if (dirAttr != null)
            {
                var path = dirAttr.Value;
                if (!Directory.Exists(path))
                {
                    Log.WriteLine(LogLevel.Info, "Directory " + path + " does not exist or is not a directory.");
                    throw new DirectoryNotFoundException("Directory " + path + " does not exist or is not a directory.");
                }
                var result = new ParseUnit();
                var sortedFiles = Directory.GetFiles(path).OrderBy(f => f);
                foreach (var filename in sortedFiles)
                    if (filename.ToLower().EndsWith(".xml"))
                    {
                        var loadedImportFile = ImportFile(filename, parentUnit);
                        result.Elements.AddRange(Pipeline(loadedImportFile).Elements);
                    }
                return result;
            }
            return parentUnit;
        }

        #endregion

        #region Processing Templates
        ParseUnit ProcessTemplates(ParseUnit inputUnit)
        {
            var templatesFiltered = LoadTemplates(inputUnit);
            var templatesExpanded = ExpandTemplates(templatesFiltered);
            return templatesExpanded;
        }

        ParseUnit LoadTemplates(ParseUnit inputUnit)
        {
            ParseUnit result = new ParseUnit(path: inputUnit.FullPath, version: inputUnit.Version);
            result.Elements = inputUnit.Elements;
            var templates = inputUnit.Elements.Where(_ => _.Name == ConfigurationConstants.Tags.TEMPLATE);
            if (!templates.All(_ => _.Attribute(ConfigurationConstants.Attrs.ID) != null))
            {
                Log.WriteLine(LogLevel.Error, "All <" + ConfigurationConstants.Tags.TEMPLATE + "> tags must have '" + ConfigurationConstants.Attrs.ID + "' set.");
                throw new FormatException("All <" + ConfigurationConstants.Tags.TEMPLATE + "> tags must have '" + ConfigurationConstants.Attrs.ID + "' set.");
            }
            var mergedTemplates = from t in templates
                                  group t.Elements() by t.Attribute(ConfigurationConstants.Attrs.ID).Value into g
                                  select CreatePair(g.Key, MergeSections(g));

            foreach (var template in mergedTemplates)
                if (m_loadedTemplates.ContainsKey(template.Key))
                    m_loadedTemplates[template.Key] = template.Value.ToList();
                else
                    m_loadedTemplates.Add(template.Key, template.Value.ToList());
            return result;
        }

        ParseUnit ExpandTemplates(ParseUnit inputUnit)
        {
            var result = new ParseUnit(path: inputUnit.FullPath, version: inputUnit.Version);
            result.Elements = inputUnit.Elements.Select(section =>
            {
                if (section.Name == ConfigurationConstants.Tags.LOCAL)
                    return ExpandTemplateForANode(section);
                if (section.Name == ConfigurationConstants.Tags.CLUSTER)
                {
                    var defaultElements = section.Elements().Where(_ => _.Name == ConfigurationConstants.Tags.DEFAULT);
                    var defaultAsTemplate = new List<IEnumerable<XElement>>();
                    if (defaultElements.Count() != 0)
                        defaultAsTemplate.Add(MergeSections(new List<IEnumerable<XElement>> { defaultElements }).First().Elements());
                    var newChildren = section.Elements().Select(e =>
                    {
                        if (e.Name == ConfigurationConstants.Tags.SERVER || e.Name == ConfigurationConstants.Tags.PROXY)
                            return ExpandTemplateForANode(e, defaultAsTemplate);
                        return e;
                    });
                    var newElem = new XElement(section.Name, section.Attributes());
                    newElem.Add(newChildren);
                    return newElem;
                }
                return section;
            }).ToList();
            return result;
        }

        XElement ExpandTemplateForANode(XElement elem, IEnumerable<IEnumerable<XElement>> sectionsBefore = null)
        {
            var templateAttr = elem.Attribute(ConfigurationConstants.Attrs.TEMPLATE);
            if (templateAttr == null)
                return elem;
            if (!m_loadedTemplates.ContainsKey(templateAttr.Value))
            {
                Log.WriteLine(LogLevel.Error, "Specified " + ConfigurationConstants.Attrs.TEMPLATE + " not found:" + templateAttr.Value + ".");
                throw new KeyNotFoundException("Specified " + ConfigurationConstants.Attrs.TEMPLATE + " not found:" + templateAttr.Value + ".");
            }
            var newElem = new XElement(elem.Name, elem.Attributes());
            var sectionsToMerge = sectionsBefore == null ? new List<IEnumerable<XElement>>() : sectionsBefore.ToList();
            sectionsToMerge.Add(m_loadedTemplates[templateAttr.Value]);
            sectionsToMerge.Add(elem.Elements());
            newElem.Add(MergeSections(sectionsToMerge));
            return newElem;
        }

        #endregion
    }
}
