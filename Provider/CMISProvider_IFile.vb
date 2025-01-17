'  ---------------------------------------------------------------------------------
'  ---------------------------------------------------------------------------------
'   Document    :  CMISProvider_IFile.vb
'   Description :  [type_description_here]
'   Created     :  2/19/2014 9:32:06 AM
'   <copyright company="ECMG">
'       Copyright (c) Enterprise Content Management Group, LLC. All rights reserved.
'       Copying or reuse without permission is strictly forbidden.
'   </copyright>
'  ---------------------------------------------------------------------------------
'  ---------------------------------------------------------------------------------

#Region "Imports"

Imports Documents.Providers
Imports Documents.Utilities

#End Region

Partial Public Class CMISProvider
  Implements IFile

#Region "IFile Implementation"

  Public Function FileDocument(ByVal lpID As String, ByVal lpFolderPath As String, ByVal lpSessionId As String) As Boolean Implements IFile.FileDocument

  End Function

  Public Function UnFileDocument(ByVal lpID As String, ByVal lpFolderPath As String, ByVal lpSessionId As String) As Boolean Implements IFile.UnFileDocument

  End Function

#End Region

End Class
