using System;


namespace Trinity.TSL
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class FOREACHAttribute : System.Attribute
    {
        public FOREACHAttribute() { }
        public FOREACHAttribute(string separator) { }
    }

    #region IF,ELIF,ELSE,END
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class IFAttribute : System.Attribute
    {
        public IFAttribute(bool value) { }
        public IFAttribute(string value) { }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class ELIFAttribute : System.Attribute
    {
        public ELIFAttribute(bool value) { }
        public ELIFAttribute(string value) { }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class ELSEAttribute : System.Attribute
    {
        public ELSEAttribute() { }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ENDAttribute : System.Attribute
    {
    }
    #endregion

    #region ____
    [AttributeUsage(AttributeTargets.All)]
    public class ____IFAttribute : System.Attribute
    {
        public ____IFAttribute(bool value) { }
        public ____IFAttribute(string value) { }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class ____ELIFAttribute : System.Attribute
    {
        public ____ELIFAttribute(bool value) { }
        public ____ELIFAttribute(string value) { }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class ____ELSEAttribute : System.Attribute
    {
        public ____ELSEAttribute() { }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ____ENDAttribute : System.Attribute
    {
    }
    #endregion

    #region ________
    [AttributeUsage(AttributeTargets.All)]
    public class ________IFAttribute : System.Attribute
    {
        public ________IFAttribute(bool value) { }
        public ________IFAttribute(string value) { }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class ________ELIFAttribute : System.Attribute
    {
        public ________ELIFAttribute(bool value) { }
        public ________ELIFAttribute(string value) { }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class ________ELSEAttribute : System.Attribute
    {
        public ________ELSEAttribute() { }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ________ENDAttribute : System.Attribute
    {
    }
    #endregion

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    class MODULE_CALLAttribute : Attribute
    {
        public MODULE_CALLAttribute(string moduleName, string moduleTarget, params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class STRUCTAttribute : System.Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class MAP_VARAttribute : System.Attribute
    {
        public string MemberOf = null;
        public MAP_VARAttribute(string from, string to) { }
    }
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class MAP_LISTAttribute : System.Attribute
    {
        public string MemberOf = null;
        public MAP_LISTAttribute(string from, string to) { }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class USE_LISTAttribute : System.Attribute
    {
        public USE_LISTAttribute(string target_list) { }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class MODULE_BEGINAttribute : System.Attribute
    {
        public MODULE_BEGINAttribute() { }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class MODULE_ENDAttribute : System.Attribute
    {
        public MODULE_ENDAttribute() { }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class TARGETAttribute : System.Attribute
    {
        public TARGETAttribute(string target) { }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class META_VARAttribute : System.Attribute
    {
        public META_VARAttribute(string type, string name) { }
        public META_VARAttribute(string type, string name, string value) { }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class METAAttribute : System.Attribute
    {
        public METAAttribute(string cmd) { }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class MUTEAttribute : System.Attribute
    {
        public MUTEAttribute() { }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class MUTE_ENDAttribute : System.Attribute
    {
        public MUTE_ENDAttribute() { }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class LITERAL_OUTPUTAttribute : System.Attribute
    {
        public LITERAL_OUTPUTAttribute(string content) { }
    }
}
