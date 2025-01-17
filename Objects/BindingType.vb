' ---------------------------------------------------------------------------------
' ---------------------------------------------------------------------------------
'  Document    :  BindingType.vb
'  Description :  [type_description_here]
'  Created     :  11/10/2011 9:52:01 AM
'  <copyright company="ECMG">
'      Copyright (c) Enterprise Content Management Group, LLC. All rights reserved.
'      Copying or reuse without permission is strictly forbidden.
'  </copyright>
' ---------------------------------------------------------------------------------
' ---------------------------------------------------------------------------------

#Region "Imports"

Imports Documents.Utilities
'Imports Ecmg.DotCMIS
'Imports DotCMIS.Client

#End Region

Public Class BindingType

#Region "Class Constants"

  Private Const ATOM_PUB As String = "AtomPub"
  Private Const WEB_SERVICES As String = "WebServices"
  Private Const CUSTOM As String = "Custom"

#End Region

#Region "Class Variables"

  Private ReadOnly mstrValueString As String = "atompub"
  Private menuTypeValue As BindingTypeEnum = BindingTypeEnum.AtomPub
  Private ReadOnly mobjCMISBindingType As BindingType

#End Region

#Region "PublicProperties"

  Public Property ValueString As String
    Get
      Return mstrValueString
    End Get
    Set(value As String)
      Try
        Select Case value.ToLower
          Case "atompub"
            If Type <> BindingTypeEnum.AtomPub Then
              Type = BindingTypeEnum.AtomPub
            End If
          Case "webservices"
            If Type <> BindingTypeEnum.WebServices Then
              Type = BindingTypeEnum.WebServices
            End If
          Case "custom"
            If Type <> BindingTypeEnum.Custom Then
              Type = BindingTypeEnum.Custom
            End If
          Case Else
            Throw New ArgumentOutOfRangeException(NameOf(value),
              String.Format("'{0}' is not a valid value for Binding Type.  Expected values are (AtomPub, WebServices or Custom).", value))
        End Select
      Catch ex As Exception
        ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
        ' Re-throw the exception to the caller
        Throw
      End Try
    End Set
  End Property

  Friend ReadOnly Property CMISBinding As BindingType
    Get
      Return mobjCMISBindingType
    End Get
  End Property

  Public Property Type As BindingTypeEnum
    Get
      Return menuTypeValue
    End Get
    Set(value As BindingTypeEnum)
      Try
        menuTypeValue = value
        Select Case value
          Case BindingTypeEnum.AtomPub
            If ValueString <> ATOM_PUB Then
              ValueString = ATOM_PUB
            End If
          Case BindingTypeEnum.WebServices
            If ValueString <> WEB_SERVICES Then
              ValueString = WEB_SERVICES
            End If
          Case BindingTypeEnum.Custom
            If ValueString <> CUSTOM Then
              ValueString = CUSTOM
            End If
          Case Else
            Throw New ArgumentOutOfRangeException(NameOf(value),
             String.Format("'{0}' is not a valid value for Binding Type.  Expected values are (AtomPub, WebServices or Custom).", value))
        End Select
      Catch ex As Exception
        ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
        ' Re-throw the exception to the caller
        Throw
      End Try
    End Set
  End Property

#End Region

#Region "Public Enumerations"

  Public Enum BindingTypeEnum

    AtomPub = 0
    WebServices = 1
    Custom = -1

  End Enum

#End Region

End Class
