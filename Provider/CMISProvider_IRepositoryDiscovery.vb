'  ---------------------------------------------------------------------------------
'  ---------------------------------------------------------------------------------
'   Document    :  CMISProvider_IRepositoryDiscovery.vb
'   Description :  [type_description_here]
'   Created     :  2/19/2014 9:06:02 AM
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
'Imports DotCMIS.Client
Imports PortCMIS.Client
Imports PortCMIS.Client.Impl

#End Region
Partial Public Class CMISProvider
	Implements IRepositoryDiscovery

	Public Function GetRepositories() As RepositoryIdentifiers Implements IRepositoryDiscovery.GetRepositories
		Try

			Dim lobjReturnList As New RepositoryIdentifiers
			Dim lobjCMISRepositories As New List(Of IRepository)

			'If IsConnected = False Then
			'  InitializeConnection()
			'End If

			'If Session Is Nothing Then
			'  Throw New Exceptions.ProviderNotInitializedException("The session is not initialized.")
			'End If

			Dim lobjSessionFactory As ISessionFactory = Impl.SessionFactory.NewInstance

			SessionParameters = InitializeSessionParameter()
			'Dim lobjSession As Session = lobjSessionFactory.CreateSession(SessionParameters)

			lobjCMISRepositories = lobjSessionFactory.GetRepositories(SessionParameters)

			For Each lobjCMISRepository As IRepository In lobjCMISRepositories
				lobjReturnList.Add(New RepositoryIdentifier(lobjCMISRepository.Id, lobjCMISRepository.Name, lobjCMISRepository.Name))
			Next

			lobjSessionFactory = Nothing
			lobjCMISRepositories = Nothing

			Return lobjReturnList

		Catch ex As Exception
			ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
			' Re-throw the exception to the caller
			Throw
		End Try
	End Function

End Class
