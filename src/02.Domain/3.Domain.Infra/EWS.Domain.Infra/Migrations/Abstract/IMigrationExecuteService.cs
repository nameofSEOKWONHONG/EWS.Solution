namespace EWS.Domain.Infra.Migrations;

public interface IMigrationExecuteService
{
    void Execute();
}

public enum ENUM_MIGRATION_TYPE
{
    Up,
    Down,
    ProcedureAndFunction
}