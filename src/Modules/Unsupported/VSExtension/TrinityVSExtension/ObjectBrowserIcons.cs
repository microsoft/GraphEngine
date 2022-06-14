using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.VSExtension
{
    //http://msdn.microsoft.com/en-us/library/y47ychfe.aspx
    enum ObjectBrowserIcon : ushort
    {
        Class            = 0,
        Delegate         = 12,
        Enum             = 18,
        Event            = 30,
        Exception        = 36,
        FieldOrVariable  = 42,
        Interface        = 48,
        Map              = 60,
        MapItem          = 66,
        MethodOrFunction = 72,
        Module           = 84,
        Namespace        = 90,
        Operator         = 96,
        Property         = 102,
        Structure        = 108,
        Template         = 114,
        Typedef          = 120,
        TrinityStruct    = 150,
        VB               = 194,
        CSharp           = 196,
        VBS              = 198,
        VC               = 199,
        FolderOpen       = 201,
        FolderClose      = 202,
        RightArrow       = 203,
        CsharpFile       = 204,
        NewFile          = 205,
        Info             = 207,
        Syn              = 208,
        Asyn             = 209,
        ForwardTelephone = 214,//well that's what I see
        BackwardTelephone= 215,
        OptionalModifier = 217,
        ExtensionMethod  = 220,
    }
}
