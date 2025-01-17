' ---------------------------------------------------------------------------------
' ---------------------------------------------------------------------------------
'  Document    :  CMISFolder.vb
'  Description :  [type_description_here]
'  Created     :  10/29/2010 9:02:56 AM
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
'Imports DotCMIS
'Imports DotCMIS.Client
Imports PortCMIS
Imports PortCMIS.Client
Imports PortCMIS.Data
Imports DocumentProviders = Documents.Providers

#End Region

Public Class CMISFolder
	Inherits CFolder

#Region "Class Variables"

	Private mobjFolderContents As New FolderContents
	Private mobjCMISFolder As Client.IFolder
	Private mobjCMISSession As Client.ISession
	Private ReadOnly mobjCmisChildren As IEnumerable(Of ICmisObject)

	Private mobjRepositoryInfo As RepositoryInfo = Nothing

#End Region

#Region "Public Properties"

	Friend ReadOnly Property RepositoryCapabilities As IRepositoryCapabilities
		Get
			If RepositoryInfo IsNot Nothing Then
				Return RepositoryInfo.Capabilities
			Else
				Return Nothing
			End If
		End Get
	End Property

	Friend Property RepositoryInfo As RepositoryInfo
		Get
			Return mobjRepositoryInfo
		End Get
		Set(value As RepositoryInfo)
			mobjRepositoryInfo = value
		End Set
	End Property

#End Region

#Region "Friend Properties"

	Friend ReadOnly Property SourceFolder As Client.IFolder
		Get
			Try
				If mobjCMISFolder IsNot Nothing Then
					Return mobjCMISFolder
				Else
					Return Nothing
				End If
			Catch ex As Exception
				ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
				' Re-throw the exception to the caller
				Throw
			End Try
		End Get
	End Property

#End Region

#Region "Private Properties"

	Private Property CMISFolder As Client.IFolder
		Get
			Return mobjCMISFolder
		End Get
		Set(value As Client.IFolder)
			mobjCMISFolder = value
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

#End Region

#Region "Constructors"

	Public Sub New()
		MyBase.New()
	End Sub

	Public Sub New(ByVal lpSession As Client.ISession,
								 ByRef lpProvider As CProvider,
								 ByVal lpMaxContentCount As Long)
		MyBase.New()
		Try
			' Assume this is for the root folder
			CMISFolder = lpSession.GetRootFolder

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Sub

	'Public Sub New(ByVal lpFolderPath As String, _
	'               ByVal lpMaxContentCount As Long)
	'  MyBase.New(lpFolderPath, lpMaxContentCount)

	'  Try
	'    mobjCMISFolder = GetFolderByPath(lpFolderPath, lpMaxContentCount)
	'    MyBase.InitializeFolderCollection(lpFolderPath)
	'    InitializeFolder()

	'  Catch ex As Exception
	'    ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
	'    ' Re-throw the exception to the caller
	'    Throw
	'  End Try

	'End Sub

	Public Sub New(ByVal lpCMISFolder As Client.IFolder,
								 ByVal lpSession As Client.ISession,
								 ByVal lpMaxContentCount As Long)

		Try
			CMISFolder = lpCMISFolder
			Session = lpSession
			Name = lpCMISFolder.Name
			MaxContentCount = lpMaxContentCount
			MyBase.InitializeFolderCollection(CMISFolder.Path)
			InitializeFolder()

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			Throw New Exception("Unable to create folder from IFolder Object", ex)
		End Try

	End Sub

	Public Sub New(ByVal lpCMISFolder As Client.IFolder,
								 ByVal lpSession As Client.ISession,
								 ByRef lpProvider As CProvider,
								 ByVal lpMaxContentCount As Long)

		Try
			CMISFolder = lpCMISFolder
			Session = lpSession
			Name = lpCMISFolder.Name
			Provider = lpProvider
			MaxContentCount = lpMaxContentCount
			MyBase.InitializeFolderCollection(CMISFolder.Path)
			InitializeFolder()

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			Throw New Exception("Unable to create folder from IFolder Object", ex)
		End Try

	End Sub

#End Region

#Region "Public Methods"

#Region "Public Overrides Methods"

	Public Overrides ReadOnly Property Contents() As Core.FolderContents
		Get
			Try
				Return mobjFolderContents
			Catch Ex As Exception
				ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
				' Re-throw the exception to the caller
				Throw
			End Try
		End Get
	End Property

	Protected Overrides Function GetFolderByPath(ByVal lpFolderPath As String, ByVal lpMaxContentCount As Long) As DocumentProviders.IFolder
		Throw New NotImplementedException
	End Function

	Public Overrides Function GetID() As String
		Throw New NotImplementedException
	End Function

	Protected Overrides Function GetPath() As String
		Throw New NotImplementedException
	End Function

	Protected Overrides Function GetSubFolderCount() As Long
		Try
			''If mobjCmisChildren Is Nothing Then
			''  mobjCmisChildren = CMISFolder.GetChildren(lobjOperationContext)
			''End If
			Return GetFolderCount(CMISFolder.GetChildren)
		Catch Ex As Exception
			ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Protected Overrides Sub InitializeFolder()
		Try
			If MaxContentCount = -1 Then
				InitializeFolderContents()
			End If
		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Sub

	Public Overrides Sub Refresh()
		Try
			InitializeFolder()
		Catch Ex As Exception
			ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Sub

	Public Overloads Overrides ReadOnly Property SubFolders() As Core.Folders
		Get

			Try
				Return GetSubFolders(True)
			Catch ex As Exception
				ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
				' Re-throw the exception to the caller
				Throw
			End Try

		End Get
	End Property

	Public Overrides Function GetSubFolders(ByVal lpGetContents As Boolean) As Core.Folders
		Dim lobjFolders As New Folders
		Dim lobjFolder As CFolder
		Try

			' Get the sub folders of the CMISFolder oject
			Dim lobjCmisFolders As IEnumerable(Of Client.ICmisObject) = CMISFolder.GetChildren.Where(Function(f) TypeOf f Is Client.IFolder)
			For Each lobjCmisFolder As Client.IFolder In lobjCmisFolders
				lobjFolder = New CMISFolder(lobjCmisFolder, Me.Session, Me.MaxContentCount)
				lobjFolders.Add(lobjFolder)
			Next

			lobjFolders.Sort()
			Return lobjFolders

		Catch Ex As Exception
			ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Function

#End Region

#End Region

#Region "Private Methods"

	Private Sub InitializeFolderContents()

		mobjFolderContents = New FolderContents

		Try

			'If Not CMISFolder Is Nothing Then

			'	Dim lobjFolderContent As FolderContent = Nothing
			'	Dim lobjCmisDocument As Client.IDocument = Nothing

			'	Dim lintSubFolderCount As Integer = 0
			'	Dim lintDocumentCount As Integer = 0

			'	Dim lobjOperationContext As IOperationContext = Session.CreateOperationContext
			'	With lobjOperationContext
			'		'.Filter.Add("cmis:document")
			'		.FilterString = "cmis:document"
			'		.CacheEnabled = True
			'		.IncludePathSegments = False
			'		.IncludePolicies = False
			'		.IncludeRelationships = Enums.IncludeRelationshipsFlag.None
			'		'.FilterString = "cmis:document"
			'	End With


			'	'For Each lobjIDocument As IDocument In CMISFolder.GetChildren
			'	'mobjCmisChildren = CMISFolder.GetChildren(lobjOperationContext)
			'	' mobjCmisChildren = CMISFolder.GetChildren

			'	' For Each lobjIObject As ICmisObject In mobjCmisChildren.Where(Function(f) f.BaseTypeId = Enums.BaseTypeId.CmisDocument)

			'	For Each lobjIObject As ICmisObject In CMISFolder.GetChildren.Where(Function(f) f.BaseTypeId = Enums.BaseTypeId.CmisDocument)
			'		'If lobjIDocument.ContentElements.Count > 0 Then
			'		'  lobjFolderContent = New FolderContent(lobjIDocument.Name, lobjIDocument.Id.ToString, CLng(lobjIDocument.ContentSize), lobjIDocument.ContentElements(0).RetrievalName, CDate(lobjIDocument.DateLastModified))

			'		'Else

			'		'  If lobjIDocument.ContentSize Is Nothing Then
			'		'    lobjFolderContent = New FolderContent(lobjIDocument.Name, lobjIDocument.Id.ToString, 0, String.Empty, CDate(lobjIDocument.DateLastModified))

			'		'  Else
			'		'    lobjFolderContent = New FolderContent(lobjIDocument.Name, lobjIDocument.Id.ToString, CLng(lobjIDocument.ContentSize), String.Empty, CDate(lobjIDocument.DateLastModified))

			'		'  End If

			'		'End If

			'		'If lobjIObject.BaseType = Document Then
			'		'  ' TODO: Add the item to the folder contents
			'		'End If

			'		''Select Case lobjIObject.BaseTypeId
			'		''  Case Enums.BaseTypeId.CmisDocument
			'		''    lintDocumentCount += 1
			'		''    Console.WriteLine(String.Format("{0} documents found.", lintDocumentCount.ToString))
			'		''    lobjCmisDocument = CType(lobjIObject, Client.IDocument)
			'		''    If Not String.IsNullOrEmpty(lobjCmisDocument.Name) Then
			'		''      Console.WriteLine(String.Format("Document Name: {0}", lobjCmisDocument.Name))
			'		''    Else
			'		''      Console.WriteLine(String.Format("Document Name: {0}", lobjCmisDocument.Paths(0)))
			'		''    End If

			'		''  Case Enums.BaseTypeId.CmisFolder
			'		''    lintSubFolderCount += 1
			'		''    Console.WriteLine(String.Format("{0} folders found.", lintSubFolderCount.ToString))
			'		''    ' Console.WriteLine(String.Format("Folder Path: {0}", CType(lobjIObject, Client.IFolder).Path))
			'		''    ' SubFolders.Add(New CMISFolder(lobjIObject, Session, Provider, MaxContentCount))
			'		''  Case Enums.BaseTypeId.CmisPolicy
			'		''    Console.WriteLine("Policy Found")
			'		''  Case Enums.BaseTypeId.CmisRelationship
			'		''    Console.WriteLine("Relationship Found")
			'		''End Select

			'		'mobjFolderContents.Add(lobjFolderContent)


			'		lintDocumentCount += 1
			'		lobjCmisDocument = CType(lobjIObject, Client.IDocument)
			'		' Dim lstrName As String = lobjCmisDocument.GetPropertyValue("name")
			'		If Not String.IsNullOrEmpty(lobjCmisDocument.Name) Then
			'			Console.WriteLine(String.Format("Document Name: {0}", lobjCmisDocument.Name))
			'		Else
			'			Console.WriteLine(String.Format("Document Name: {0}", lobjCmisDocument.Paths(0)))
			'		End If
			'		lobjFolderContent = New FolderContent(lobjCmisDocument.Name, lobjCmisDocument.Id, Me)
			'		mobjFolderContents.Add(lobjFolderContent)
			'	Next

			'End If

			If RepositoryCapabilities IsNot Nothing Then
				If RepositoryCapabilities.IsGetDescendantsSupported = False Then
					Exit Sub
				End If
			End If
			mobjFolderContents = New FolderContents

			Dim lobjCmisFolderChildren As IItemEnumerable(Of ICmisObject) = mobjCMISFolder.GetChildren()

			For Each lobjCmisFolderChild As ICmisObject In lobjCmisFolderChildren
				If TypeOf lobjCmisFolderChild Is Client.IFolder Then
					'Beep()
				ElseIf TypeOf lobjCmisFolderChild Is Client.IDocument Then
					'Dim lobjFolderContent As FolderContent = GetFolderContent(lobjCmisFolderChild, lobjCmisFolderChild)
					Dim lobjFolderContent As FolderContent = GetFolderContent(lobjCmisFolderChild)
					mobjFolderContents.Add(lobjFolderContent)
					'Else
					'	Beep()
				End If

			Next

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			Throw New Exception("Unable to Initialize Folder Contents", ex)
		End Try

	End Sub

	Private Function GetFolderContent(lpDocument As Client.IDocument) As FolderContent
		Try

#If NET8_0_OR_GREATER Then
			ArgumentNullException.ThrowIfNull(lpDocument)
#Else
      If lpDocument Is Nothing Then
        Throw New ArgumentNullException(NameOf(lpDocument))
      End If
#End If

			Dim lobjFolderContent As FolderContent

			lobjFolderContent = New FolderContent(lpDocument.Name) With {
				.ParentFolderId = Me.Id
			}

			For Each lobjProperty As Client.IProperty In lpDocument.Properties
				lobjFolderContent.Properties.Add(lobjProperty.LocalName, lobjProperty.Value)
			Next

			Return lobjFolderContent

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Function

	Private Shared Function GetFolderCount(ByVal lpFolders As IEnumerable(Of ICmisObject)) As Integer
		Try
			'Return lpFolders.Where(Function(f) TypeOf f Is Client.IFolder).Count
			Dim lintFolderCount As Integer
			For Each lobjFolderCandidate As ICmisObject In lpFolders
				If lobjFolderCandidate.BaseTypeId = Enums.BaseTypeId.CmisFolder Then
					lintFolderCount += 1
				End If
			Next

			Return lintFolderCount

		Catch Ex As Exception
			ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Function

#End Region

End Class
