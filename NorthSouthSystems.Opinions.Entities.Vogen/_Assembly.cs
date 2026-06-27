using Vogen;

[assembly: VogenDefaults(
    disableStackTraceRecordingInDebug: true,
    explicitlySpecifyTypeInValueObject: true,
    isInitializedMethodGeneration: IsInitializedMethodGeneration.Omit)]