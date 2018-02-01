using Trinity.TSL;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    [TARGET("NTSL")]
    [MAP_LIST("t_cell", "node->cellList")]
    [MAP_VAR("t_cell_name", "name")]
    [MAP_VAR("t_ushort", "Trinity::Codegen::GetCellTypeOffset()")]
    [MAP_VAR("t_ushort_2", "Trinity::Codegen::GetCellTypeOffset() + 1 + GET_ITERATOR_VALUE()")]
    public enum CellType: ushort
    {
        Undefined = __meta.t_ushort,
        /*FOREACH("")*/
        t_cell_name = __meta.t_ushort_2,
        /*END*/
    }
}
