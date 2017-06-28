Option Strict On
Option Explicit On

Imports System.ServiceModel
Imports System.ServiceModel.Discovery


Namespace RoutinesLibrary.Net.WCF

    ''' <summary>
    ''' Provides methods to search WCF services
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SearcherWCFServices

        ''' <summary>
        ''' Search all services given a service type
        ''' </summary>
        ''' <param name="serviceType">Service type for search</param>
        ''' <param name="searchTime">Search duration in seconds</param>
        ''' <returns>List of founded services</returns>
        Public Function SearchServices(serviceType As Type, Optional searchTime As Integer = 3) As List(Of EndpointAddress)
            Dim endpoints As New List(Of EndpointAddress)
            Dim discovery As New DiscoveryClient(New UdpDiscoveryEndpoint)
            Dim findCriteria As New FindCriteria(serviceType)
            findCriteria.Duration = TimeSpan.FromSeconds(searchTime)
            Dim resp As FindResponse = discovery.Find(findCriteria)

            For Each ep As EndpointDiscoveryMetadata In resp.Endpoints
                endpoints.Add(ep.Address)
            Next
            discovery.Close()

            Return endpoints
        End Function

    End Class

End Namespace
