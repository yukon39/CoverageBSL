namespace com.github.yukon39.CoverageBSL.AddIn.Converters
{
    internal enum CoverageModuleKind
    {
        Unknown,
        OrdinaryApplicationModule,
        ManagedApplicationModule,
        ExternalConnectionModule,
        SessionModule,
        CommonModule,
        ObjectModule,
        ManagerModule,
        RecordSetModule,
        ValueManagerModule,
        SettingsManagerModule,
        FormModule,
        CommandModule
    }

    internal static class CoverageModuleKindExtensions
    {
        internal static CoverageModuleKind FromPropertyId(this string propertyId) =>
            propertyId.ToLowerInvariant() switch
            {
                "a78d9ce3-4e0c-48d5-9863-ae7342eedf94" => CoverageModuleKind.OrdinaryApplicationModule,
                "d22e852a-cf8a-4f77-8ccb-3548e7792bea" => CoverageModuleKind.ManagedApplicationModule,
                "a4a9c1e2-1e54-4c7f-af06-4ca341198fac" => CoverageModuleKind.ExternalConnectionModule,
                "9b7bbbae-9771-46f2-9e4d-2489e0ffc702" => CoverageModuleKind.SessionModule,
                "d5963243-262e-4398-b4d7-fb16d06484f6" => CoverageModuleKind.CommonModule,
                "a637f77f-3840-441d-a1c3-699c8c5cb7e0" => CoverageModuleKind.ObjectModule,
                "d1b64a2c-8078-4982-8190-8f81aefda192" => CoverageModuleKind.ManagerModule,
                "9f36fd70-4bf4-47f6-b235-935f73aab43f" => CoverageModuleKind.RecordSetModule,
                "3e58c91f-9aaa-4f42-8999-4baf33907b75" => CoverageModuleKind.ValueManagerModule,
                "0c8cad23-bf8c-468e-b49e-12f1927c048b" => CoverageModuleKind.SettingsManagerModule,
                "32e087ab-1491-49b6-aba7-43571b41ac2b" => CoverageModuleKind.FormModule,
                "078a6af8-d22c-4248-9c33-7e90075a3d2c" => CoverageModuleKind.CommandModule,
                _ => CoverageModuleKind.Unknown
            };
    }
}
