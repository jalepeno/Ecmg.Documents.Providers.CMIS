'  ---------------------------------------------------------------------------------
'  ---------------------------------------------------------------------------------
'   Document    :  CMISProvider_ICreate.vb
'   Description :  [type_description_here]
'   Created     :  2/19/2014 9:20:43 AM
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
  Implements ICreate


#Region "ICreate Implementation"

  Public Function AddDocument(ByVal lpDocument As Document, ByVal lpFolderPath As String, ByVal lpSessionId As String) As String Implements ICreate.AddDocument
    Throw New NotImplementedException
  End Function

  Public Function CreateDocumentInstance(ByVal lpDocumentClassName As String, ByVal lpSessionId As String) As Document Implements ICreate.CreateDocumentInstance
    Throw New NotImplementedException
  End Function

#End Region

End Class
