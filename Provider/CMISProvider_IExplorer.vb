'  ---------------------------------------------------------------------------------
'  ---------------------------------------------------------------------------------
'   Document    :  CMISProvider_IExplorer.vb
'   Description :  [type_description_here]
'   Created     :  2/19/2014 8:57:31 AM
'   <copyright company="ECMG">
'       Copyright (c) Enterprise Content Management Group, LLC. All rights reserved.
'       Copying or reuse without permission is strictly forbidden.
'   </copyright>
'  ---------------------------------------------------------------------------------
'  ---------------------------------------------------------------------------------

#Region "Imports"

Imports Documents.Core
Imports Documents.Providers
Imports Documents.Utilities

#End Region

Partial Public Class CMISProvider
  Implements IExplorer

#Region "IExplorer Implementation"

  Public ReadOnly Property GetFolderByID(ByVal lpFolderID As String, ByVal lpFolderLevels As Integer, ByVal lpMaxContentCount As Integer) As IFolder Implements IExplorer.GetFolderByID
    Get
      Throw New NotImplementedException
    End Get
  End Property

  Public ReadOnly Property GetFolderContentsByID(ByVal lpFolderID As String, ByVal lpMaxContentCount As Integer) As FolderContents Implements IExplorer.GetFolderContentsByID
    Get
      Throw New NotImplementedException
    End Get
  End Property

  Public ReadOnly Property HasSubFolders(ByVal lpFolderID As String) As Boolean Implements IExplorer.HasSubFolders
    Get
      Throw New NotImplementedException
    End Get
  End Property

  Public ReadOnly Property IsFolderValid(ByVal lpFolderID As String) As Boolean Implements IExplorer.IsFolderValid
    Get
      Throw New NotImplementedException
    End Get
  End Property

  Public ReadOnly Property RootFolder() As IFolder Implements IExplorer.RootFolder
    Get
      Try
        If mobjRootFolder Is Nothing Then
          InitializeRootFolder()
        End If
        Return mobjRootFolder
      Catch ex As Exception
        ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
        ' Re-throw the exception to the caller
        Throw
      End Try
    End Get
  End Property

  Public Overrides ReadOnly Property Search() As ISearch Implements IExplorer.Search
    Get
      Throw New NotImplementedException
    End Get
  End Property

#End Region


End Class
