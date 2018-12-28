﻿Imports System.Threading
Imports Algo2TradeCore
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports NLog

Public Class OHLStrategyInstrument
    Inherits StrategyInstrument

#Region "Logging and Status Progress"
    Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

    Public Sub New(ByVal associatedInstrument As IInstrument, ByVal associatedParentStrategy As Strategy, ByVal canceller As CancellationTokenSource)
        MyBase.New(associatedInstrument, associatedParentStrategy, canceller)
        _APIAdapter = New ZerodhaAdapter(ParentStrategy.ParentContoller, _cts)
        AddHandler _APIAdapter.Heartbeat, AddressOf OnHeartbeat
        AddHandler _APIAdapter.WaitingFor, AddressOf OnWaitingFor
        AddHandler _APIAdapter.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
        AddHandler _APIAdapter.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
    End Sub
    Public Overrides Function ToString() As String
        Return String.Format("{0}_{1}", ParentStrategy.ToString, TradableInstrument.ToString)
    End Function
    Public Overrides Async Function RunDirectAsync() As Task
        logger.Debug("{0}->RunDirectAsync, parameters:None", Me.ToString)
        Try
            While Me.ParentStrategy.ParentContoller.APIConnection Is Nothing
                logger.Debug("Waiting for fresh token:{0}", TradableInstrument.InstrumentIdentifier)
                Await Task.Delay(500).ConfigureAwait(False)
            End While
            Dim triggerRecevied As Tuple(Of Boolean, Trigger) = Await IsTriggerReachedAsync().ConfigureAwait(False)
            If triggerRecevied IsNot Nothing AndAlso triggerRecevied.Item1 = True Then
                _APIAdapter.SetAPIAccessToken(Me.ParentStrategy.ParentContoller.APIConnection.AccessToken)
                Dim allTrades As IEnumerable(Of ITrade) = Await _APIAdapter.GetAllTradesAsync().ConfigureAwait(False)
            End If
        Catch ex As Exception
            logger.Debug("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^")
            logger.Error(ex.ToString)
        End Try
    End Function
    Public Overrides Async Function IsTriggerReachedAsync() As Task(Of Tuple(Of Boolean, Trigger))
        logger.Debug("{0}->IsTriggerReachedAsync, parameters:None", Me.ToString)
        Await Task.Delay(0).ConfigureAwait(False)
        Dim ret As Tuple(Of Boolean, Trigger) = Nothing
        If _LastTick IsNot Nothing AndAlso _LastTick.Timestamp IsNot Nothing AndAlso _LastTick.Open = _LastTick.Low Then
            ret = New Tuple(Of Boolean, Trigger)(True, New Trigger() With {.Category = Trigger.TriggerType.PriceMatched, .Description = String.Format("O=L,({0})", _LastTick.Open)})
        ElseIf _LastTick IsNot Nothing AndAlso _LastTick.Timestamp IsNot Nothing AndAlso _LastTick.Open = _LastTick.High Then
            ret = New Tuple(Of Boolean, Trigger)(True, New Trigger() With {.Category = Trigger.TriggerType.PriceMatched, .Description = String.Format("O=H,({0})", _LastTick.Open)})
        End If
        'TO DO: Remove the below hard coding
        ret = New Tuple(Of Boolean, Trigger)(True, Nothing)
        Return ret
    End Function
    Public Overrides Async Function ProcessTickAsync(ByVal tickData As ITick) As Task
        'logger.Debug("ProcessTickAsync, tickData:{0}", Utilities.Strings.JsonSerialize(tickData))
        Await Task.Delay(0).ConfigureAwait(False)
        Select Case tickData.Broker
            Case APISource.Zerodha
                Console.WriteLine(CType(tickData, ZerodhaTick).LastPrice & "-" & CType(tickData, ZerodhaTick).Timestamp)
        End Select
    End Function
End Class
