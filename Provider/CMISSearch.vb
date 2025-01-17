' ---------------------------------------------------------------------------------
' ---------------------------------------------------------------------------------
'  Document    :  CMISSearch.vb
'  Description :  [type_description_here]
'  Created     :  10/29/2010 9:03:14 AM
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

#End Region

Public Class CMISSearch
  Inherits CSearch

#Region "Class Variables"

#End Region

#Region "Public Properties"

  Public Overrides ReadOnly Property DefaultQueryTarget As String
    Get
      Throw New NotImplementedException
    End Get
  End Property

#End Region

#Region "Constructors"

#End Region

#Region "Public Methods"

#Region "Public Overrides Methods"

  Public Overloads Overrides Function Execute() As Core.SearchResultSet
    Throw New NotImplementedException
  End Function

  Public Overloads Overrides Function Execute(ByVal Args As Arguments.SearchArgs) As Core.SearchResultSet
    Throw New NotImplementedException
  End Function

  Public Overrides Function SimpleSearch(ByVal Args As Arguments.SimpleSearchArgs) As System.Data.DataTable
    Throw New NotImplementedException
  End Function

#End Region

#End Region

#Region "Private Methods"

#End Region

End Class