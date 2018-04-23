# How to enforce task dependencies in the Gantt view


<p>This example illustrates the use of the <a href="http://documentation.devexpress.com/#WPF/DevExpressXpfSchedulerSchedulerControl_AllowAppointmentConflictstopic"><u>AllowAppointmentConflicts</u></a> event to implement restrictions on appointment editing so that appointments linked with a dependency  follow the rules of this dependency. <br />
If two appointments are linked with a <a href="http://documentation.devexpress.com/#CoreLibraries/DevExpressXtraSchedulerAppointmentDependencyTypeEnumtopic"><u>Finish-to-Start</u></a> dependency, then <strong>Finish</strong> value of the parent appointment should always be less or equal the <strong>Start </strong>value of the dependent appointment. To ensure this behavior, we analyze dependencies for conflicting appointments.<br />
The <a href="http://documentation.devexpress.com/#CoreLibraries/DevExpressXtraSchedulerAppointmentDependencyBaseCollection_GetDependenciesByDependentIdtopic"><u>GetDependenciesByDependentId</u></a> and <a href="http://documentation.devexpress.com/#CoreLibraries/DevExpressXtraSchedulerAppointmentDependencyBaseCollection_GetDependenciesByParentIdtopic"><u>GetDependenciesByParentId</u></a> methods are used to obtain dependency collections within the event handler.</p>

<br/>


