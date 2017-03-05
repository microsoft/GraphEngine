using Trinity.TSL;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    [TARGET("NTSL")]
    [MAP_LIST("t_cell", "node->cellList")]
    [MAP_VAR("t_cell_name", "name")]
    [MAP_VAR("t_ushort", "GET_ITERATOR_VALUE() + 1")]
    public enum CellTypeEnum : ushort
    {
        Undefined = 0,
        /*FOREACH("")*/
        t_cell_name = __meta.t_ushort,
        /*END*/
    }
}
