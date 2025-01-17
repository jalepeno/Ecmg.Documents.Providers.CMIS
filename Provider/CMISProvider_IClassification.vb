'  ---------------------------------------------------------------------------------
'  ---------------------------------------------------------------------------------
'   Document    :  CMISProvider_IClassification.vb
'   Description :  [type_description_here]
'   Created     :  2/19/2014 9:02:38 AM
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
  Implements IClassification

#Region "IClassification Implementation"

  Public ReadOnly Property ContentProperties() As ClassificationProperties Implements IClassification.ContentProperties
    Get
      Try

        If mobjProperties Is Nothing Then
          mobjProperties = GetAllContentProperties()
        End If

        Return mobjProperties

      Catch ex As Exception
        ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
        ' Re-throw the exception to the caller
        Throw
      End Try
    End Get
  End Property

  Public ReadOnly Property DocumentClasses() As DocumentClasses Implements IClassification.DocumentClasses
    Get
      Try

        If mobjDocumentClasses Is Nothing Then
          mobjDocumentClasses = GetDocumentClasses()
        End If

        Return mobjDocumentClasses

      Catch ex As Exception
        ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
        ' Re-throw the exception to the caller
        Throw
      End Try
    End Get
  End Property

  Public ReadOnly Property DocumentClass(ByVal lpDocumentClassName As String) As DocumentClass Implements IClassification.DocumentClass
    Get
      Try
        Return DocumentClasses(lpDocumentClassName)
      Catch ex As Exception
        ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
        ' Re-throw the exception to the caller
        Throw
      End Try
    End Get
  End Property

#End Region

End Class
