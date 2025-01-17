'  ---------------------------------------------------------------------------------
'  ---------------------------------------------------------------------------------
'   Document    :  CMISProvider_ICopy.vb
'   Description :  [type_description_here]
'   Created     :  2/19/2014 9:16:42 AM
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
  Implements ICopy

#Region "ICopy Implementation"

  Public Function GetDocument(ByVal lpId As String, ByVal lpGetContents As Boolean, ByVal lpStorageType As Content.StorageTypeEnum, ByVal lpDestinationFolder As String, ByVal SessionId As String) As Document Implements ICopy.GetDocument
    Throw New NotImplementedException
  End Function

  Public Function GetDocumentXml(ByVal lpId As String, ByVal lpGetContents As Boolean, ByVal lpStorageType As Content.StorageTypeEnum, ByVal lpDestinationFolder As String, ByVal SessionId As String) As String Implements ICopy.GetDocumentXml
    Throw New NotImplementedException
  End Function

#End Region

End Class
