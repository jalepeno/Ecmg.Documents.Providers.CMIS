'  ---------------------------------------------------------------------------------
'  ---------------------------------------------------------------------------------
'   Document    :  CMISProvider_IRename.vb
'   Description :  [type_description_here]
'   Created     :  2/19/2014 9:19:09 AM
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

Public Partial Class CMISProvider
  Implements IRename

#Region "IRename Implementation"

  Public Function RenameDocument(ByVal lpID As String, ByVal lpNewName As String, ByVal SessionId As String) As Boolean Implements IRename.RenameDocument
    Throw New NotImplementedException
  End Function

#End Region

End Class
