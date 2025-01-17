' ---------------------------------------------------------------------------------
' ---------------------------------------------------------------------------------
'  Document    :  CMISProvider.vb
'  Description :  [type_description_here]
'  Created     :  10/29/2010 9:02:18 AM
'  <copyright company="ECMG">
'      Copyright (c) Enterprise Content Management Group, LLC. All rights reserved.
'      Copying or reuse without permission is strictly forbidden.
'  </copyright>
' ---------------------------------------------------------------------------------
' ---------------------------------------------------------------------------------

#Region "Imports"

Imports Documents
Imports Documents.Arguments
Imports Documents.Core
Imports Documents.Exceptions
Imports Documents.Providers
Imports Documents.Utilities
Imports Documents.Utilities.ApplicationLogging
'Imports DotCMIS
Imports System.Reflection
'Imports DotCMIS.Client
'Imports DotCMIS.Client.Impl
Imports Documents.Licensing
'Imports DotCMIS.Exceptions
'Imports DotCMIS.Data
Imports PortCMIS.Data
Imports PortCMIS
Imports PortCMIS.Client
Imports PortCMIS.Client.Impl
Imports PortCMIS.Exceptions
Imports DocumentProviders = Documents.Providers

#End Region

Public Class CMISProvider
	Inherits CProvider

#Region "Class Constants"

	Private Const FOLDER_DELIMITER As String = "/"
	Private Const PROVIDER_NAME As String = "CMIS Provider"
	Private Const PROVIDER_SYSTEM_TYPE As String = "CMIS"
	Private Const PROVIDER_COMPANY_NAME As String = "Apache"
	Private Const PROVIDER_PRODUCT_NAME As String = "CMIS"
	Private Const PROVIDER_PRODUCT_VERSION As String = "1.1"

	Public Const BINDING_TYPE_NAME As String = "BindingType"
	Public Const URL_NAME As String = "Url"
	Public Const REPOSITORY_NAME As String = "Repository"

#End Region

#Region "Class Variables"

	' This is where you declare the system identifiers 
	' These constants are duplicated in all classes implementing IProvider.
	' The actual values are stored in the project's app.config file
	Private mobjSystem As ProviderSystem = Nothing

	Private mobjRootFolder As CFolder = Nothing
	Private mobjSearch As CSearch = New CMISSearch

	Private mobjCMISSession As Client.ISession
	Private mobjRepositoryService As Binding.Services.IRepositoryService

	Private mstrBindingType As String = PortCMIS.BindingType.AtomPub
	Private mstrUrl As String = String.Empty
	Private mstrRepositoryName As String = String.Empty
	Private mobjSessionParameter As IDictionary(Of String, String)

	Private mstrUserName As String = String.Empty

	Private mobjRepositoryCapabilities As IRepositoryCapabilities = Nothing
	Private mstrPassword As String = String.Empty
	Private mobjRepositoryInfo As RepositoryInfo = Nothing
	Private mobjRepositoryIdentifiers As RepositoryIdentifiers = Nothing

	' For IClassification
	Private mobjDocumentClasses As DocumentClasses
	Private mobjRequestedDocumentClasses As DocumentClasses
	Private mobjProperties As ClassificationProperties
	Private mobjRequestedProperties As ClassificationProperties

	Private mblnCanUnFile As Nullable(Of Boolean) = Nothing

#End Region

#Region "Public Properties"

	Public Overrides ReadOnly Property ProviderSystem() As ProviderSystem
		Get
			Try
				If mobjSystem Is Nothing Then
					mobjSystem = New ProviderSystem(PROVIDER_NAME,
																	PROVIDER_SYSTEM_TYPE,
																	PROVIDER_COMPANY_NAME,
																	PROVIDER_PRODUCT_NAME,
																	PROVIDER_PRODUCT_VERSION)
				End If
				Return mobjSystem
			Catch ex As Exception
				ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
				' Re-throw the exception to the caller
				Throw
			End Try
		End Get
	End Property

	''' <summary>
	''' Gets the folder delimiter used by a specific repository.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Overrides ReadOnly Property FolderDelimiter() As String
		Get
			Return FOLDER_DELIMITER
		End Get
	End Property

	''' <summary>
	''' Gets a value specifying whether or 
	''' not the repository expects a leading 
	''' delimiter for all folder paths.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Overrides ReadOnly Property LeadingFolderDelimiter() As Boolean
		Get
			Return True
		End Get
	End Property

#End Region

#Region "Private Properties"

	Private ReadOnly Property RepositoryIdentifiers As RepositoryIdentifiers
		Get
			Try
				If mobjRepositoryIdentifiers Is Nothing Then
					mobjRepositoryIdentifiers = GetRepositories()
				End If
				Return mobjRepositoryIdentifiers
			Catch ex As Exception
				ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
				' Re-throw the exception to the caller
				Throw
			End Try
		End Get
	End Property

	Private Property BindingType As String
		Get
			Return mstrBindingType
		End Get
		Set(value As String)
			mstrBindingType = value
		End Set
	End Property

	Friend ReadOnly Property Capabilities As IRepositoryCapabilities
		Get
			If Info IsNot Nothing Then
				Return Info.Capabilities
			Else
				Return Nothing
			End If
		End Get
	End Property

	Friend Property Info As RepositoryInfo
		Get
			Return mobjRepositoryInfo
		End Get
		Set(value As RepositoryInfo)
			mobjRepositoryInfo = value
		End Set
	End Property

	Private Property Session As Client.ISession
		Get
			Return mobjCMISSession
		End Get
		Set(value As Client.ISession)
			mobjCMISSession = value
		End Set
	End Property

	Private Property SessionParameters As IDictionary(Of String, String)
		Get
			Return mobjSessionParameter
		End Get
		Set(value As IDictionary(Of String, String))
			mobjSessionParameter = value
		End Set
	End Property

	Private ReadOnly Property RepositoryService As Binding.Services.IRepositoryService
		Get
			Try
				If mobjRepositoryService Is Nothing AndAlso Session IsNot Nothing Then
					mobjRepositoryService = Session.Binding.GetRepositoryService
				End If
				Return mobjRepositoryService
			Catch ex As Exception
				ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
				'   Re-throw the exception to the caller
				Throw
			End Try
		End Get
	End Property

	Private ReadOnly Property CanUnfile As Boolean
		Get
			Try
				If mblnCanUnFile Is Nothing Then
					mblnCanUnFile = GetCanUnfile()
				End If
				Return mblnCanUnFile
			Catch ex As Exception
				ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
				'   Re-throw the exception to the caller
				Throw
			End Try
		End Get
	End Property

	Private Function GetCanUnfile() As Boolean
		Try
			'Dim lobjRepositoryInfo As IRepositoryInfo = RepositoryService.GetRepositoryInfo(RepositoryName, Nothing)
			'If lobjRepositoryInfo IsNot Nothing Then
			'  Return lobjRepositoryInfo.Capabilities.IsUnfilingSupported
			'Else
			'  Return False
			'End If
			Return Session.RepositoryInfo.Capabilities.IsUnfilingSupported
		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			'   Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Public Overrides Property UserName As String
		Get
			If String.IsNullOrEmpty(mstrUserName) Then
				' Try to get it from the provider property.
				RefreshPropertyFromProviderProperties(USER)
			End If
			Return mstrUserName
		End Get
		Set(value As String)
			mstrUserName = value
		End Set
	End Property

	Public Overrides Property Password As String
		Get
			If String.IsNullOrEmpty(mstrPassword) Then
				' Try to get it from the provider property.
				RefreshPropertyFromProviderProperties("Password")
			End If
			Return mstrPassword
		End Get
		Set(value As String)
			mstrPassword = value
		End Set
	End Property

	Public Property Url As String
		Get
			Try
				If String.IsNullOrEmpty(mstrUrl) Then
					' Try to get it from the provider property.
					RefreshPropertyFromProviderProperties(URL_NAME)
				End If
				Return mstrUrl
			Catch ex As Exception
				ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
				' Re-throw the exception to the caller
				Throw
			End Try
		End Get
		Set(value As String)
			mstrUrl = value
		End Set
	End Property

	Private Property RepositoryName As String
		Get
			Return mstrRepositoryName
		End Get
		Set(value As String)
			mstrRepositoryName = value
		End Set
	End Property

#End Region

#Region "Constructors"

	Public Sub New()

		MyBase.New()

		Try
			AddProperties()
			MyBase.ExportPath = "%CtsDocsPath%\Exports"
			' <Removed by: Ernie at: 9/29/2014-11:23:03 AM on machine: ERNIE-THINK>
			'       MyBase.ImportPath = "%CtsDocsPath%\Imports"
			' </Removed by: Ernie at: 9/29/2014-11:23:03 AM on machine: ERNIE-THINK>
			'mobjSearch = CreateSearch()

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try

	End Sub

	Public Sub New(ByVal lpConnectionString As String)

		MyBase.New(lpConnectionString)

		Try
			AddProperties()
			ParseConnectionString()
			mobjSearch = CreateSearch()

			'Login(SystemName, ServerName, UserName, Password)
		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try

	End Sub

#End Region

#Region "Provider Identification"

	Private Sub AddProperties()

		' Add the properties here that you want to show up in the 'Create Data Source' dialog.

		Try

			' Add the 'UserName' property
			MyBase.ProviderProperties.Add(New ProviderProperty(USER, GetType(System.String), False, , , , , True))

			' Add the 'Password' property
			MyBase.ProviderProperties.Add(New ProviderProperty(PWD, GetType(System.String), False, , , , , True))

			' Add the 'BindingType' property
			MyBase.ProviderProperties.Add(New ProviderProperty(BINDING_TYPE_NAME, GetType(System.String), True, "AtomPub", ,
																												 "Specifies with binding type to use for the connection.", True))

			' Add the 'Url' property
			MyBase.ProviderProperties.Add(New ProviderProperty(URL_NAME, GetType(System.String), True, , , , , True))

			' Add the 'Repository' property
			MyBase.ProviderProperties.Add(New ProviderProperty(REPOSITORY_NAME, GetType(System.String), True, , ,
																												 "Specifies which repository to connect to.", True, True))

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try

	End Sub

#End Region

#Region "Public Methods"

#Region "Public Overrides Methods"

	'Public Overrides ReadOnly Property Feature As FeatureEnum
	'	Get
	'		Try
	'			Return FeatureEnum.CMISProvider
	'		Catch Ex As Exception
	'			ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
	'			' Re-throw the exception to the caller
	'			Throw
	'		End Try
	'	End Get
	'End Property

#Region "Connect"

	Public Overrides Sub Connect()

		Try
			InitializeConnection()

		Catch ex As Exception
			LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			IsConnected = False
			Throw New ApplicationException("Connection Failed: " & Helper.FormatCallStack(ex))
		End Try

	End Sub

	Public Overrides Sub Connect(ByVal ConnectionString As String)

		Try
			MyBase.ConnectionString = ConnectionString
			ParseConnectionString()
			InitializeConnection()

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			IsConnected = False
			Throw New ApplicationException("Connection Failed: " & Helper.FormatCallStack(ex))
		End Try

	End Sub

	Public Overrides Sub Connect(ByVal ContentSource As ContentSource)

		Try
			InitializeProvider(ContentSource)
			InitializeProperties()
			InitializeConnection()
			IsConnected = True

		Catch RepoUnavailableEx As RepositoryNotAvailableException
			ApplicationLogging.LogException(RepoUnavailableEx, Reflection.MethodBase.GetCurrentMethod)
			IsConnected = False
			Throw
		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			IsConnected = False
			Throw New ApplicationException("Connection Failed: " & Helper.FormatCallStack(ex))
		End Try

	End Sub

#End Region

#End Region

	''' <summary>
	''' Gets the available values to set for the specified provider property.
	''' </summary>
	''' <param name="lpProviderProperty"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Overrides Function GetAvailableValues(ByVal lpProviderProperty As ProviderProperty) As IEnumerable(Of String)

		Try

			If lpProviderProperty.SupportsValueList = False Then
				Throw New Exceptions.PropertyDoesNotSupportValueListException(lpProviderProperty)
			End If

			Dim lobjReturnValues As List(Of String) = MyBase.GetAvailableValues(lpProviderProperty)

			Select Case lpProviderProperty.PropertyName

				Case BINDING_TYPE_NAME
					lobjReturnValues.Clear()
					With lobjReturnValues
						.Add("AtomPub")
						.Add("WebServices")
						.Add("Custom")
					End With

				Case REPOSITORY_NAME
					lobjReturnValues.Clear()

					For Each lobjObjectStoreIdentifier As RepositoryIdentifier In RepositoryIdentifiers
						lobjReturnValues.Add(lobjObjectStoreIdentifier.Name)
					Next

					lobjReturnValues.Sort()
			End Select

			Return lobjReturnValues

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try

	End Function

	Public Overrides Function GetFolder(ByVal lpFolderPath As String, ByVal lpMaxContentCount As Long) As DocumentProviders.IFolder
		Try
			Throw New NotImplementedException
		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Public Overrides Function CreateSearch() As ISearch
		Try
			Throw New NotImplementedException
		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Function

#End Region

#Region "Private Methods"

	Private Sub InitializeRootFolder()
		Try
			mobjRootFolder = New CMISFolder(Session.GetRootFolder, Session, Me, -1) With {
				.InvisiblePassThrough = False,
				.Provider = Me,
				.Name = "Root"
			}
		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Sub

	Private Function InitializeSessionParameter(Optional ByVal lpRepositoryId As String = Nothing) As IDictionary(Of String, String)
		Try

			Dim lobjParameterMap As IDictionary(Of String, String) = New Dictionary(Of String, String)

			Select Case BindingType
				Case PortCMIS.BindingType.AtomPub
					lobjParameterMap.Add(SessionParameter.BindingType, BindingType)
					lobjParameterMap.Add(SessionParameter.AtomPubUrl, Url)
					lobjParameterMap.Add(SessionParameter.User, UserName)
					lobjParameterMap.Add(SessionParameter.Password, Password)
					If Not String.IsNullOrEmpty(lpRepositoryId) Then
						lobjParameterMap.Add(SessionParameter.RepositoryId, lpRepositoryId)
					End If

					'  Case CMIS.BindingType.BindingTypeEnum.WebServices
					'    Throw New NotImplementedException
					'    lobjParameterMap.Add(SessionParameter.BindingType, BindingType.ValueString)
					'    lobjParameterMap.Add(SessionParameter.WebServicesRepositoryService, "http://<host>/<RepositoryServiceWSDL>")
					'    lobjParameterMap.Add(SessionParameter.WebServicesAclService, "http://<host>/<RepositoryServiceWSDL>")
					'    lobjParameterMap.Add(SessionParameter.WebServicesDiscoveryService, "http://<host>/<RepositoryServiceWSDL>")
					'    lobjParameterMap.Add(SessionParameter.WebServicesMultifilingService, "http://<host>/<RepositoryServiceWSDL>")
					'    lobjParameterMap.Add(SessionParameter.WebServicesNavigationService, "http://<host>/<RepositoryServiceWSDL>")
					'    lobjParameterMap.Add(SessionParameter.WebServicesObjectService, "http://<host>/<RepositoryServiceWSDL>")
					'    lobjParameterMap.Add(SessionParameter.WebServicesPolicyService, "http://<host>/<RepositoryServiceWSDL>")
					'    lobjParameterMap.Add(SessionParameter.WebServicesRelationshipService, "http://<host>/<RepositoryServiceWSDL>")
					'    lobjParameterMap.Add(SessionParameter.WebServicesVersioningService, "http://<host>/<RepositoryServiceWSDL>")
					'    lobjParameterMap.Add(SessionParameter.User, UserName)
					'    lobjParameterMap.Add(SessionParameter.Password, Password)

				Case Else
					'    Throw New NotImplementedException
			End Select

			Return lobjParameterMap

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Private Function InitializeConnection() As Boolean
		Try

			Dim lobjSessionFactory As ISessionFactory = SessionFactory.NewInstance

			Dim lstrRepositoryId As String = RepositoryIdentifiers.GetRepositoryId(Me.RepositoryName)
			SessionParameters = InitializeSessionParameter(lstrRepositoryId)

			If Not SessionParameters.ContainsKey(SessionParameter.RepositoryId) Then
				SessionParameters.Add(SessionParameter.RepositoryId, lstrRepositoryId)
			End If

			Session = (lobjSessionFactory.CreateSession(SessionParameters))

		Catch CmisEx As CmisRuntimeException
			ApplicationLogging.LogException(CmisEx, Reflection.MethodBase.GetCurrentMethod)
			If CmisEx.Message.ToLower.Contains("unavailable") Then
				Me.SetState(ProviderConnectionState.Unavailable)
				Throw New RepositoryNotAvailableException(Me.RepositoryName, String.Format("Connection Failed: {0}", CmisEx.Message), CmisEx)
			Else
				' Re-throw the exception to the caller
				Throw
			End If
		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Private Sub InitializeProperties()

		Try

			For Each lobjProviderProperty As ProviderProperty In ProviderProperties

				Select Case lobjProviderProperty.PropertyName

					Case BINDING_TYPE_NAME
						BindingType = lobjProviderProperty.PropertyValue

					Case REPOSITORY_NAME
						RepositoryName = lobjProviderProperty.PropertyValue

					Case URL_NAME
						Url = lobjProviderProperty.PropertyValue

					Case USER
						UserName = lobjProviderProperty.PropertyValue

					Case PWD
						Password = lobjProviderProperty.PropertyValue

					Case EXPORT_PATH
						ExportPath = lobjProviderProperty.PropertyValue

						' <Removed by: Ernie at: 9/29/2014-11:23:30 AM on machine: ERNIE-THINK>
						'           Case IMPORT_PATH
						'             ImportPath = lobjProviderProperty.PropertyValue
						' </Removed by: Ernie at: 9/29/2014-11:23:30 AM on machine: ERNIE-THINK>

				End Select

			Next

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try

	End Sub

	Private Sub ParseConnectionString()


		Try
			RepositoryName = Helper.GetInfoFromString(ConnectionString, REPOSITORY_NAME)

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			Throw New ArgumentException("Argument not provided", REPOSITORY_NAME, ex)
		End Try

		Try
			Url = Helper.GetInfoFromString(ConnectionString, URL_NAME)

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			Throw New ArgumentException("Argument not provided", URL_NAME, ex)
		End Try

		Try
			mstrBindingType = Helper.GetInfoFromString(ConnectionString, BINDING_TYPE_NAME)

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			Throw New ArgumentException("Valid argument not provided", BINDING_TYPE_NAME, ex)
		End Try

		Try
			UserName = Helper.GetInfoFromString(ConnectionString, "UserName")

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			Throw New ArgumentException("Argument not provided", "UserName", ex)
		End Try

		Try
			Password = Helper.GetInfoFromString(ConnectionString, "Password")

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			Throw New ArgumentException("Argument not provided", "Password", ex)
		End Try

		Try
			ExportPath = Helper.GetInfoFromString(ConnectionString, EXPORT_PATH)

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			ExportPath = "%CtsDocsPath%\Exports"
		End Try

		' <Removed by: Ernie at: 9/29/2014-11:23:40 AM on machine: ERNIE-THINK>
		'     Try
		'       ImportPath = Helper.GetInfoFromString(ConnectionString, IMPORT_PATH)
		' 
		'     Catch ex As Exception
		'       ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
		'       ImportPath = "%CtsDocsPath%\Imports"
		'     End Try
		' </Removed by: Ernie at: 9/29/2014-11:23:40 AM on machine: ERNIE-THINK>

	End Sub

#End Region

#Region "Private Methods"

	Private Function GetFolderFromPath(lpPath As String) As String
		Try
			Return IO.Path.GetDirectoryName(lpPath).Replace("\", "/")
		Catch Ex As Exception
			ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Private Function GetDocumentClasses() As DocumentClasses
		Try
			Dim lobjDocumentClasses As New DocumentClasses
			Return GetDocumentClasses(Nothing, lobjDocumentClasses)
		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			'   Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Private Function GetDocumentClasses(lpParentClass As ITypeDefinition, ByRef lpCurrentDocumentClasses As DocumentClasses) As DocumentClasses
		Try

			Dim lobjDocumentTypeDefinition As ITypeDefinition

			If lpParentClass Is Nothing Then
				lobjDocumentTypeDefinition = RepositoryService.GetTypeDefinition(Session.RepositoryInfo.Id, "cmis:document", Nothing)
			Else
				lobjDocumentTypeDefinition = lpParentClass
			End If

			If lobjDocumentTypeDefinition IsNot Nothing Then
				lpCurrentDocumentClasses.Add(GetDocumentClassFromTypeDefinition(lobjDocumentTypeDefinition))
			End If

			Dim lobjChildTypes As ITypeDefinitionList = RepositoryService.GetTypeChildren(
				Session.RepositoryInfo.Id, lobjDocumentTypeDefinition.Id, True, Nothing, Nothing, Nothing)

			For Each lobjChildTypeDefinition As ITypeDefinition In lobjChildTypes.List
				lpCurrentDocumentClasses = GetDocumentClasses(lobjChildTypeDefinition, lpCurrentDocumentClasses)
			Next

			Return lpCurrentDocumentClasses

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			'   Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Private Function GetDocumentClassFromTypeDefinition(lpTypeDefinition As ITypeDefinition) As DocumentClass
		Try

			Return New DocumentClass(lpTypeDefinition.QueryName, GetTypeProperties(lpTypeDefinition), lpTypeDefinition.Id, lpTypeDefinition.DisplayName)

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			'   Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Private Function GetAllContentProperties() As ClassificationProperties
		Dim lobjProperties As New ClassificationProperties

		'Dim lobjDocumentTypeDefinition As ITypeDefinition
		Try

			'lobjDocumentTypeDefinition = RepositoryService.GetTypeDefinition(Session.RepositoryInfo.Id, "cmis:document", Nothing)

			'   lobjProperties.AddRange(GetTypeProperties(lobjDocumentTypeDefinition))

			'Dim lobjChildTypes As ITypeDefinitionList = RepositoryService.GetTypeChildren( _
			'	Session.RepositoryInfo.Id, lobjDocumentTypeDefinition.Id, True, Nothing, Nothing, Nothing)

			'   ' TODO: Make this a true recursion
			'   For Each lobjObjectType As ITypeDefinition In lobjChildTypes.List
			'     'Debug.Print(lobjObjectType.DisplayName)
			'     lobjProperties.AddRange(GetTypeProperties(lobjObjectType))
			'   Next

			'   lobjProperties.Sort()

			If Me.DocumentClasses IsNot Nothing Then
				For Each lobjDocumentClass As DocumentClass In Me.DocumentClasses
					lobjProperties.AddRange(lobjDocumentClass.Properties)
				Next
				lobjProperties.Sort()
			End If

			Return lobjProperties

		Catch Ex As Exception
			ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Private Function GetTypeProperties(lpType As ITypeDefinition) As ClassificationProperties
		Try

			Dim lobjReturnProperties As New ClassificationProperties

			For Each lobjCmisProperty As IPropertyDefinition In lpType.PropertyDefinitions
				lobjReturnProperties.Add(CmisPropertyDefinitionToClassificationProperty(lobjCmisProperty))
			Next

			' lobjReturnProperties.Sort()

			Return lobjReturnProperties

		Catch Ex As Exception
			ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Private Function CmisPropertyDefinitionToClassificationProperty(lpPropertyDefinition As IPropertyDefinition) As ClassificationProperty
		Try
			Dim lobjReturnProperty As Core.ClassificationProperty

			If lpPropertyDefinition.Cardinality = Enums.Cardinality.Single Then
				lobjReturnProperty = ClassificationPropertyFactory.Create(CmisPropertyTypeToCtsPropertyType(lpPropertyDefinition.PropertyType),
																										lpPropertyDefinition.DisplayName, lpPropertyDefinition.Id, Cardinality.ecmSingleValued)

			Else
				lobjReturnProperty = ClassificationPropertyFactory.Create(CmisPropertyTypeToCtsPropertyType(lpPropertyDefinition.PropertyType),
																						lpPropertyDefinition.DisplayName, lpPropertyDefinition.Id, Cardinality.ecmMultiValued)
			End If

			With lobjReturnProperty
				.Description = lpPropertyDefinition.Description
				.IsInherited = lpPropertyDefinition.IsInherited
				.IsRequired = lpPropertyDefinition.IsRequired
				.Settability = UpdateabilityToSettability(lpPropertyDefinition.Updatability)
				'If lpPropertyDefinition.IsOpenChoice = False Then
				'	Beep()
				'Else
				'	Beep()
				'End If

				'	Get the type specific details
				Select Case lpPropertyDefinition.PropertyType
					Case Enums.PropertyType.Boolean
						Dim lobjPropertyDefinition As IPropertyBooleanDefinition = CType(lpPropertyDefinition, IPropertyBooleanDefinition)
						If lobjPropertyDefinition.Cardinality = Enums.Cardinality.Single Then
							If lobjPropertyDefinition.DefaultValue IsNot Nothing Then
								.DefaultValue = lobjPropertyDefinition.DefaultValue.FirstOrDefault
							End If
						End If
						If lobjPropertyDefinition.Choices IsNot Nothing Then
							lobjReturnProperty.ChoiceList = CreateChoiceListFromCmisChoices(String.Format("{0} Choices",
								lobjPropertyDefinition.DisplayName), lobjPropertyDefinition.Choices)
						End If
					Case Enums.PropertyType.DateTime
						Dim lobjPropertyDefinition As IPropertyDateTimeDefinition = CType(lpPropertyDefinition, IPropertyDateTimeDefinition)
						If lobjPropertyDefinition.Cardinality = Enums.Cardinality.Single Then
							If lobjPropertyDefinition.DefaultValue IsNot Nothing Then
								.DefaultValue = lobjPropertyDefinition.DefaultValue.FirstOrDefault
							End If
						End If
						If lobjPropertyDefinition.Choices IsNot Nothing Then
							lobjReturnProperty.ChoiceList = CreateChoiceListFromCmisChoices(String.Format("{0} Choices",
								lobjPropertyDefinition.DisplayName), lobjPropertyDefinition.Choices)
						End If
					Case Enums.PropertyType.Decimal
						Dim lobjPropertyDefinition As IPropertyDecimalDefinition = CType(lpPropertyDefinition, IPropertyDecimalDefinition)
						If lobjPropertyDefinition.Cardinality = Enums.Cardinality.Single Then
							If lobjPropertyDefinition.DefaultValue IsNot Nothing Then
								.DefaultValue = lobjPropertyDefinition.DefaultValue.FirstOrDefault
							End If
						End If
						CType(lobjReturnProperty, ClassificationDoubleProperty).MinValue = lobjPropertyDefinition.MinValue
						CType(lobjReturnProperty, ClassificationDoubleProperty).MaxValue = lobjPropertyDefinition.MaxValue
						If lobjPropertyDefinition.Choices IsNot Nothing Then
							lobjReturnProperty.ChoiceList = CreateChoiceListFromCmisChoices(String.Format("{0} Choices",
								lobjPropertyDefinition.DisplayName), lobjPropertyDefinition.Choices)
						End If
					Case Enums.PropertyType.Html
						Dim lobjPropertyDefinition As IPropertyHtmlDefinition = CType(lpPropertyDefinition, IPropertyHtmlDefinition)
						If lobjPropertyDefinition.Cardinality = Enums.Cardinality.Single Then
							If lobjPropertyDefinition.DefaultValue IsNot Nothing Then
								.DefaultValue = lobjPropertyDefinition.DefaultValue.FirstOrDefault
							End If
						End If
						If lobjPropertyDefinition.Choices IsNot Nothing Then
							lobjReturnProperty.ChoiceList = CreateChoiceListFromCmisChoices(String.Format("{0} Choices",
								lobjPropertyDefinition.DisplayName), lobjPropertyDefinition.Choices)
						End If
					Case Enums.PropertyType.Id
						Dim lobjPropertyDefinition As IPropertyIdDefinition = CType(lpPropertyDefinition, IPropertyIdDefinition)
						If lobjPropertyDefinition.Cardinality = Enums.Cardinality.Single Then
							If lobjPropertyDefinition.DefaultValue IsNot Nothing Then
								.DefaultValue = lobjPropertyDefinition.DefaultValue.FirstOrDefault
							End If
						End If
						If lobjPropertyDefinition.Choices IsNot Nothing Then
							lobjReturnProperty.ChoiceList = CreateChoiceListFromCmisChoices(String.Format("{0} Choices",
								lobjPropertyDefinition.DisplayName), lobjPropertyDefinition.Choices)
						End If
					Case Enums.PropertyType.Integer
						Dim lobjPropertyDefinition As IPropertyIntegerDefinition = CType(lpPropertyDefinition, IPropertyIntegerDefinition)
						If lobjPropertyDefinition.Cardinality = Enums.Cardinality.Single Then
							If lobjPropertyDefinition.DefaultValue IsNot Nothing Then
								.DefaultValue = lobjPropertyDefinition.DefaultValue.FirstOrDefault
							End If
						End If
						CType(lobjReturnProperty, ClassificationLongProperty).MinValue = lobjPropertyDefinition.MinValue
						CType(lobjReturnProperty, ClassificationLongProperty).MaxValue = lobjPropertyDefinition.MaxValue
						If lobjPropertyDefinition.Choices IsNot Nothing Then
							lobjReturnProperty.ChoiceList = CreateChoiceListFromCmisChoices(String.Format("{0} Choices",
								lobjPropertyDefinition.DisplayName), lobjPropertyDefinition.Choices)
						End If
					Case Enums.PropertyType.String
						Dim lobjPropertyDefinition As IPropertyStringDefinition = CType(lpPropertyDefinition, IPropertyStringDefinition)
						If lobjPropertyDefinition.Cardinality = Enums.Cardinality.Single Then
							If lobjPropertyDefinition.DefaultValue IsNot Nothing Then
								.DefaultValue = lobjPropertyDefinition.DefaultValue.FirstOrDefault
							End If
						End If
						CType(lobjReturnProperty, ClassificationStringProperty).MaxLength = lobjPropertyDefinition.MaxLength
						If lobjPropertyDefinition.Choices IsNot Nothing Then
							lobjReturnProperty.ChoiceList = CreateChoiceListFromCmisChoices(String.Format("{0} Choices",
										lobjPropertyDefinition.DisplayName), lobjPropertyDefinition.Choices)
						End If
					Case Enums.PropertyType.Uri
						Dim lobjPropertyDefinition As IPropertyUriDefinition = CType(lpPropertyDefinition, IPropertyUriDefinition)
						If lobjPropertyDefinition.Cardinality = Enums.Cardinality.Single Then
							If lobjPropertyDefinition.DefaultValue IsNot Nothing Then
								.DefaultValue = lobjPropertyDefinition.DefaultValue.FirstOrDefault
							End If
						End If
						If lobjPropertyDefinition.Choices IsNot Nothing Then
							lobjReturnProperty.ChoiceList = CreateChoiceListFromCmisChoices(String.Format("{0} Choices",
								lobjPropertyDefinition.DisplayName), lobjPropertyDefinition.Choices)
						End If
				End Select
			End With

			Return lobjReturnProperty

			'lpPropertyDefinition.

		Catch Ex As Exception
			ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Private Function CreateChoiceListFromCmisChoices(lpChoiceListNameName As String, lpChoices As IList) As ChoiceLists.ChoiceList
		Try
			Return CreateChoiceListFromCmisChoices(lpChoiceListNameName, lpChoices, Nothing)
		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			'   Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Private Function CreateChoiceListFromCmisChoices(lpChoiceListName As String, lpChoices As IList, ByRef lpParentChoiceList As ChoiceLists.ChoiceList) As ChoiceLists.ChoiceList
		Try
			Dim lobjChoiceList As ChoiceLists.ChoiceList
			Dim lobjChoiceItem As ChoiceLists.ChoiceItem
			Dim lstrValue As String = String.Empty

			If lpParentChoiceList IsNot Nothing Then
				lobjChoiceList = lpParentChoiceList
			Else
				lobjChoiceList = New ChoiceLists.ChoiceList
				If Not String.IsNullOrEmpty(lpChoiceListName) Then
					lobjChoiceList.Name = lpChoiceListName
				End If
			End If

			For Each lobjCmisChoice As Object In lpChoices
				If lobjCmisChoice.Value IsNot Nothing Then
					For Each lobjValue As Object In lobjCmisChoice.Value
						lstrValue = lobjValue.ToString()
						If Helper.ObjectContainsProperty(lobjCmisChoice, "DisplayName") Then
							lobjChoiceItem = New ChoiceLists.ChoiceItem(lobjCmisChoice.DisplayName)
						Else
							lobjChoiceItem = New ChoiceLists.ChoiceItem(lstrValue)
						End If
						lobjChoiceItem.Id = lstrValue
						lobjChoiceList.ChoiceValues.Add(lobjChoiceItem)
					Next
				End If
				If lobjCmisChoice.Choices IsNot Nothing AndAlso lobjCmisChoice.Choices.Count > 0 Then
					lobjChoiceList = CreateChoiceListFromCmisChoices(lobjCmisChoice.Choices, lobjChoiceList)
				End If
			Next

			Return lobjChoiceList

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			'   Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Private Function UpdateabilityToSettability(lpUpdatability As Enums.Updatability) As ClassificationProperty.SettabilityEnum
		Try
			Select Case lpUpdatability
				Case Enums.Updatability.ReadWrite
					Return ClassificationProperty.SettabilityEnum.READ_WRITE
				Case Enums.Updatability.ReadOnly
					Return ClassificationProperty.SettabilityEnum.READ_ONLY
				Case Enums.Updatability.OnCreate
					Return ClassificationProperty.SettabilityEnum.SETTABLE_ONLY_ON_CREATE
				Case Enums.Updatability.WhenCheckedOut
					Return ClassificationProperty.SettabilityEnum.SETTABLE_ONLY_BEFORE_CHECKIN
			End Select
		Catch Ex As Exception
			ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Private Function CmisPropertiesToCtsProperties(lpCmisProperties As IList(Of Client.IProperty)) As Core.IProperties
		Try
			Dim lobjReturnProperties As Core.IProperties = New ECMProperties
			Dim lobjCtsProperty As Core.IProperty

			For Each lobjCmisProperty As Client.IProperty In lpCmisProperties
				lobjCtsProperty = CreateCtsProperty(lobjCmisProperty)
				lobjReturnProperties.Add(lobjCtsProperty)
			Next

			Return lobjReturnProperties

		Catch Ex As Exception
			ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Private Function CreateCtsProperty(lpCmisProperty As Client.IProperty) As Core.IProperty
		Try
			Dim lobjReturnProperty As Core.IProperty

			If lpCmisProperty.IsMultiValued Then
				lobjReturnProperty = PropertyFactory.Create(CmisPropertyTypeToCtsPropertyType(lpCmisProperty.PropertyType),
																										lpCmisProperty.DisplayName, Cardinality.ecmMultiValued)
				If lpCmisProperty.Values IsNot Nothing Then
					For Each lobjValue As Object In lpCmisProperty.Values
						lobjReturnProperty.Values.Add(lobjValue)
					Next
				End If
			Else
				lobjReturnProperty = PropertyFactory.Create(CmisPropertyTypeToCtsPropertyType(lpCmisProperty.PropertyType),
																										lpCmisProperty.DisplayName, Cardinality.ecmSingleValued)
				lobjReturnProperty.Value = lpCmisProperty.Value
			End If

			Return lobjReturnProperty

		Catch Ex As Exception
			ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Private Function CtsPropertiesToCmisPropertyDictionary(lpProperties As Core.IProperties) As Dictionary(Of String, Object)
		Try
			Dim lobjPropertyDictionary As New Dictionary(Of String, Object)
			Dim lobjAvailableProperties As ClassificationProperties = Me.ContentProperties
			Dim lobjTargetProperty As ClassificationProperty = Nothing


			For Each lobjProperty As Core.IProperty In lpProperties
				'lobjPropertyDictionary.Add(PropertyIds.Name, )

				'If lobjAvailableProperties.Contains(lobjProperty.Name) Then
				'  lobjTargetProperty = lobjAvailableProperties(lobjProperty.Name)
				'  If lobjTargetProperty Is Nothing Then
				'    Continue For
				'  End If
				'  Select Case lobjTargetProperty.Settability
				'    Case ClassificationProperty.SettabilityEnum.READ_WRITE, ClassificationProperty.SettabilityEnum.SETTABLE_ONLY_ON_CREATE
				'      If lobjTargetProperty.Cardinality = Cardinality.ecmSingleValued Then
				'        lobjPropertyDictionary.Add(lobjProperty.Name, lobjProperty.Value)
				'      Else

				'      End If
				'  End Select
				'End If

				' Reset the target property to eliminate false positives from previous iterations
				lobjTargetProperty = Nothing

				If lobjAvailableProperties.Contains(lobjProperty.Name) Then
					'lobjTargetProperty = lobjAvailableProperties(lobjProperty.Name)
					lobjTargetProperty = lobjAvailableProperties.ItemByName(lobjProperty.Name)
				ElseIf lobjAvailableProperties.Contains(lobjProperty.SystemName) Then
					'lobjTargetProperty = lobjAvailableProperties(lobjProperty.SystemName)
					lobjTargetProperty = lobjAvailableProperties.ItemByName(lobjProperty.SystemName)
				End If

				If lobjTargetProperty Is Nothing Then
					Continue For
				End If

				Select Case lobjTargetProperty.Settability
					Case ClassificationProperty.SettabilityEnum.READ_WRITE, ClassificationProperty.SettabilityEnum.SETTABLE_ONLY_ON_CREATE
						If lobjTargetProperty.Cardinality = Cardinality.ecmSingleValued Then
							' lobjPropertyDictionary.Add(lobjProperty.Name, lobjProperty.Value)
							lobjPropertyDictionary.Add(lobjTargetProperty.SystemName, lobjProperty.Value)
						Else

						End If
				End Select

			Next

			Return lobjPropertyDictionary

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Private Function CmisPropertyTypeToCtsPropertyType(lpCmisPropertyType As Enums.PropertyType) As PropertyType
		Try
			Select Case lpCmisPropertyType
				Case Enums.PropertyType.String
					Return PropertyType.ecmString
				Case Enums.PropertyType.Boolean
					Return PropertyType.ecmBoolean
				Case Enums.PropertyType.DateTime
					Return PropertyType.ecmDate
				Case Enums.PropertyType.Decimal
					Return PropertyType.ecmDouble
				Case Enums.PropertyType.Html
					Return PropertyType.ecmHtml
				Case Enums.PropertyType.Id
					Return PropertyType.ecmString
				Case Enums.PropertyType.Integer
					Return PropertyType.ecmLong
				Case Enums.PropertyType.Uri
					Return PropertyType.ecmUri
				Case Else
					Return PropertyType.ecmUndefined
			End Select
		Catch Ex As Exception
			ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Function

#End Region

End Class