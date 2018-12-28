﻿Imports System.Text.RegularExpressions
Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Controller
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports NLog

Public Class OHLStrategy
    Inherits Strategy
#Region "Logging and Status Progress"
    Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

    Public Sub New(ByVal associatedParentController As APIStrategyController, ByVal canceller As CancellationTokenSource)
        MyBase.New(associatedParentController, canceller)
    End Sub
    ''' <summary>
    ''' This function will fill the instruments based on the stratgey used and also create the workers
    ''' </summary>
    ''' <param name="allInstruments"></param>
    ''' <returns></returns>
    Public Overrides Async Function CreateTradableStrategyInstrumentsAsync(ByVal allInstruments As IEnumerable(Of IInstrument)) As Task(Of Boolean)
        If allInstruments IsNot Nothing AndAlso allInstruments.Count > 0 Then
            logger.Debug("CreateTradableStrategyInstrumentsAsync, allInstruments.Count:{0}", allInstruments.Count)
        Else
            logger.Debug("CreateTradableStrategyInstrumentsAsync, allInstruments.Count:Nothing or 0")
        End If
        Dim ret As Boolean = False
        Dim tradableInstrumentsAsPerStrategy As List(Of IInstrument) = Nothing
        Await Task.Delay(0).ConfigureAwait(False)
        logger.Debug("Starting to fill strategy specific instruments, strategy:{0}", Me.ToString)
        If allInstruments IsNot Nothing AndAlso allInstruments.Count > 0 Then
            'Get all the futures instruments
            Dim futureAllInstruments = allInstruments.Where(Function(x)
                                                                Return x.InstrumentType = "FUT" AndAlso x.Exchange = "MCX"
                                                                'Return x.InstrumentType = "FUT" AndAlso x.Exchange = "NFO"
                                                            End Function)
            If futureAllInstruments IsNot Nothing AndAlso futureAllInstruments.Count > 0 Then
                'For Each runningFutureAllInstrument In futureAllInstruments
                '    Dim coreInstrumentName As String = Regex.Replace(runningFutureAllInstrument.TradingSymbol, "[0-9]+[A-Z]+FUT", "")
                '    If coreInstrumentName IsNot Nothing Then
                '        Dim cashInstrumentToAdd = allInstruments.Where(Function(x)
                '                                                           Return x.TradingSymbol = coreInstrumentName
                '                                                       End Function).FirstOrDefault
                '        If cashInstrumentToAdd IsNot Nothing AndAlso cashInstrumentToAdd.TradingSymbol IsNot Nothing AndAlso (tradableInstrumentsAsPerStrategy Is Nothing OrElse (tradableInstrumentsAsPerStrategy IsNot Nothing AndAlso tradableInstrumentsAsPerStrategy.Find(Function(x)

                '                                                                                                                                                                                                                                                                   Return x.InstrumentIdentifier = cashInstrumentToAdd.InstrumentIdentifier
                '                                                                                                                                                                                                                                                               End Function) Is Nothing)) Then

                '            ret = True
                '            If tradableInstrumentsAsPerStrategy Is Nothing Then tradableInstrumentsAsPerStrategy = New List(Of IInstrument)
                '            tradableInstrumentsAsPerStrategy.Add(cashInstrumentToAdd)
                '        End If
                '    End If
                'Next
                For Each runningFutureAllInstrument In futureAllInstruments
                    ret = True
                    If tradableInstrumentsAsPerStrategy Is Nothing Then tradableInstrumentsAsPerStrategy = New List(Of IInstrument)
                    tradableInstrumentsAsPerStrategy.Add(runningFutureAllInstrument)
                Next

            End If
        End If

        If tradableInstrumentsAsPerStrategy IsNot Nothing AndAlso tradableInstrumentsAsPerStrategy.Count > 0 Then
            'tradableInstrumentsAsPerStrategy = tradableInstrumentsAsPerStrategy.Take(5).ToList
            'Now create the strategy tradable instruments
            Dim retTradableStrategyInstruments As List(Of OHLStrategyInstrument) = Nothing
            logger.Debug("Creating strategy tradable instruments, _tradableInstruments.count:{0}", tradableInstrumentsAsPerStrategy.Count)
            For Each runningTradableInstrument In tradableInstrumentsAsPerStrategy
                If retTradableStrategyInstruments Is Nothing Then retTradableStrategyInstruments = New List(Of OHLStrategyInstrument)
                Dim runningTradableStrategyInstrument As New OHLStrategyInstrument(runningTradableInstrument, Me, _cts)
                AddHandler runningTradableStrategyInstrument.Heartbeat, AddressOf OnHeartbeat
                AddHandler runningTradableStrategyInstrument.WaitingFor, AddressOf OnWaitingFor
                AddHandler runningTradableStrategyInstrument.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                AddHandler runningTradableStrategyInstrument.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete

                retTradableStrategyInstruments.Add(runningTradableStrategyInstrument)
            Next
            TradableStrategyInstruments = retTradableStrategyInstruments
        Else
            Throw New ApplicationException(String.Format("Cannot run this strategy as no strategy instruments could be created from the tradable instruments, stratgey:{0}", Me.ToString))
        End If

        Return ret
    End Function
    ''' <summary>
    ''' This will create the required number of instrument workers based on the already filled tradable instruments.
    ''' It will also trigger the RunDirect method if the common condition for trigger for all instruments as per this strategy is satisfied
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Async Function ExecuteAsync() As Task
        logger.Debug("ExecuteAsync, parameters:Nothing")

        'To fire any time based common calls to the strategy instruments
        While True
            Dim triggerRecevied As Tuple(Of Boolean, Trigger) = Await IsTriggerReachedAsync().ConfigureAwait(False)
            If triggerRecevied IsNot Nothing AndAlso triggerRecevied.Item1 = True Then
                If TradableStrategyInstruments IsNot Nothing AndAlso TradableStrategyInstruments.Count > 0 Then
                    For Each runningTradableStrategyInstrument In TradableStrategyInstruments
                        runningTradableStrategyInstrument.RunDirectAsync()
                    Next
                End If
            End If
            Await Task.Delay(1001).ConfigureAwait(False)
        End While
    End Function
    Public Overrides Async Function SubscribeAsync(ByVal usableTicker As APITicker) As Task
        logger.Debug("SubscribeAsync, usableTicker:{0}", usableTicker.ToString)
        If TradableStrategyInstruments IsNot Nothing AndAlso TradableStrategyInstruments.Count > 0 Then
            Dim runningInstrumentIdentifiers As List(Of String) = Nothing
            For Each runningTradableStrategyInstruments In TradableStrategyInstruments
                If runningInstrumentIdentifiers Is Nothing Then runningInstrumentIdentifiers = New List(Of String)
                runningInstrumentIdentifiers.Add(runningTradableStrategyInstruments.TradableInstrument.InstrumentIdentifier)
            Next
            Await usableTicker.SubscribeAsync(runningInstrumentIdentifiers).ConfigureAwait(False)
        End If
    End Function
    Public Overrides Async Function IsTriggerReachedAsync() As Task(Of Tuple(Of Boolean, Trigger))
        logger.Debug("IsTriggerReachedAsync, parameters:Nothing")
        Await Task.Delay(0).ConfigureAwait(False)
        Dim ret As Tuple(Of Boolean, Trigger) = Nothing
        Dim currentTime As Date = Now
        Dim compareTime As TimeSpan = Nothing
        TimeSpan.TryParse("15:32:30", compareTime)
        If Utilities.Time.IsTimeEqualTillSeconds(currentTime, compareTime) Then
            ret = New Tuple(Of Boolean, Trigger)(True,
                                                 New Trigger() With
                                                 {.Category = Trigger.TriggerType.Timebased,
                                                 .Description = String.Format("Time reached:{0}", currentTime.ToString("HH:mm:ss"))})
        End If
        'TO DO: remove the below hard coding
        ret = New Tuple(Of Boolean, Trigger)(True, Nothing)
        Return ret
    End Function
    Public Overrides Function ToString() As String
        Return Me.GetType().Name
    End Function
End Class
