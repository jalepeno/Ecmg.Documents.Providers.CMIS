'  ---------------------------------------------------------------------------------
'  ---------------------------------------------------------------------------------
'   Document    :  CMISProvider_IVersion.vb
'   Description :  [type_description_here]
'   Created     :  2/19/2014 9:27:38 AM
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
  Implements IVersion

#Region "IVersion Implementation"

  Public Function CancelCheckoutDocument(ByVal lpID As String, ByVal SessionId As String) As Boolean Implements IVersion.CancelCheckoutDocument

  End Function

  Public Function CheckInDocument(ByVal lpID As String, ByVal lpContentPath As String, ByVal lpAsMajorVersion As Boolean, ByVal lpSessionId As String) As Boolean Implements IVersion.CheckInDocument

  End Function

  Public Function CheckInDocument(ByVal lpID As String, ByVal lpContentPath() As String, ByVal lpAsMajorVersion As Boolean, ByVal lpSessionId As String) As Boolean Implements IVersion.CheckInDocument

  End Function

  Public Function CheckOutDocument(ByVal lpID As String, ByVal lpDestinationFolder As String, ByRef lpOutputFileNames() As String, ByVal lpSessionID As String) As Boolean Implements IVersion.CheckOutDocument

  End Function

#End Region

End Class
