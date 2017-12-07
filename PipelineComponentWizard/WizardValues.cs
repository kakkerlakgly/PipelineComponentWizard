namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    /// <summary>
    /// List of constants to find values in the namevaluecollection
    /// </summary>
    public static class WizardValues
    {
        /// <summary>
        /// defines the version of the component, as entered by the user
        /// </summary>
        public const string ComponentVersion = "ComponentVersion";
        /// <summary>
        /// defines the classname, as entered by the user
        /// </summary>
        public const string ClassName = "ClassName";
        /// <summary>
        /// defines the description (single-line) of the component, as entered by the user
        /// </summary>
        public const string ComponentDescription = "ComponentDescription";
        /// <summary>
        /// defines the namespace in which the component should reside, as entered by the user
        /// </summary>
        public const string Namespace = "Namespace";
        /// <summary>
        /// defines the component name, as entered by the user
        /// </summary>
        public const string ComponentName = "ComponentName";
        /// <summary>
        /// defines the icon this component will display within the toolbox of Visual Studio
        /// </summary>
        public const string ComponentIcon = "ComponentIcon";
        /// <summary>
        /// defines the type of pipeline component the user wishes to have generated
        /// </summary>
        public const string PipelineType = "PipelineType";
        /// <summary>
        /// defines the stage in which the user would like it's generated
        /// pipeline component to reside
        /// </summary>
        public const string ComponentStage = "ComponentStage";
        /// <summary>
        /// defines whether the user wants to let the wizard implement the IProbeMessage
        /// interface, which allows the pipeline component to determine for itself whether
        /// it's interested in processing an inbound message
        /// </summary>
        public const string ImplementIProbeMessage = "ImplementIProbeMessage";
        /// <summary>
        /// defines the programming languages in which the pipeline component should
        /// be implemented, as choosen by the user
        /// </summary>
        public const string ImplementationLanguage = "ImplementationLanguage";
    }
}