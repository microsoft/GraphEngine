using Trinity.TSL;
using Trinity.Utilities;

namespace Trinity.TSL
{
    internal static class MessageAccessorTemplate
    {
        internal static string GenerateCode()
        {
            CodeWriter src = new CodeWriter();
            SpecificationScript script = SpecificationScript.CurrentScript;

            MessageAccessorCollection msgs = new MessageAccessorCollection(script);

            foreach (var msg in msgs.MessageAccessorTuples)
            {
                src += MessageReaderWriterTemplate.GenerateStructMessageReader(msg);
                src += MessageReaderWriterTemplate.GenerateStructMessageWriter(msg);
                src += "\r\n";
            }

            foreach (var msg in msgs.CellMessageAccessorTuples)
            {
                string      name          = msg.Name;
                Field   	cell_id_field = new Field();
                Modifier    optional_mod  = Modifier.Optional;

                msg.Name                = name + "_Message";
                cell_id_field.Modifiers = new System.Collections.Generic.List<Modifier> { optional_mod };
                cell_id_field.Name      = "CellId"; //TODO remove the hard coded string
                cell_id_field.Type      = new AtomType(TokenId.LongType);
                cell_id_field.Attribute = new AttributeDescriptor();
                msg.Fields.Add(cell_id_field);

                src += StructCodeTemplate.GenerateStructCode(msg, forCell: false);
                src += StructCodeTemplate.GenerateAccessorCode(msg);

                msg.Name = name;

                src += MessageReaderWriterTemplate.GenerateCellMessageReader(msg);
                src += MessageReaderWriterTemplate.GenerateCellMessageWriter(msg);

                src += "\r\n";

                msg.Fields.Remove(cell_id_field);

            }
            return src;
        }
    }
}

