'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.239
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------
Namespace GanttRestrictions.Properties

    <Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")>
    Friend NotInheritable Partial Class Settings
        Inherits Global.System.Configuration.ApplicationSettingsBase

        Private Shared defaultInstance As GanttRestrictions.Properties.Settings = CType((Global.System.Configuration.ApplicationSettingsBase.Synchronized(New GanttRestrictions.Properties.Settings())), GanttRestrictions.Properties.Settings)

        Public Shared ReadOnly Property [Default] As Settings
            Get
                Return GanttRestrictions.Properties.Settings.defaultInstance
            End Get
        End Property

        <Global.System.Configuration.ApplicationScopedSettingAttribute()>
        <Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>
        <Global.System.Configuration.SpecialSettingAttribute(Global.System.Configuration.SpecialSetting.ConnectionString)>
        <Global.System.Configuration.DefaultSettingValueAttribute("Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\GantTest01.mdf;Integrat" & "ed Security=True;User Instance=True")>
        Public ReadOnly Property GantTest01ConnectionString As String
            Get
                Return(CStr((Me("GantTest01ConnectionString"))))
            End Get
        End Property
    End Class
End Namespace
