﻿Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entities
Imports NLog

Namespace Strategies
    Public MustInherit Class StrategyInstrument
        Implements INotifyPropertyChanged
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Protected Sub NotifyPropertyChanged(ByVal p As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(p))
        End Sub

#Region "Events/Event handlers"
        Public Event DocumentDownloadCompleteEx(ByVal source As List(Of Object))
        Public Event DocumentRetryStatusEx(ByVal currentTry As Integer, ByVal totalTries As Integer, ByVal source As List(Of Object))
        Public Event HeartbeatEx(ByVal msg As String, ByVal source As List(Of Object))
        Public Event WaitingForEx(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String, ByVal source As List(Of Object))
        'The below functions are needed to allow the derived classes to raise the above two events
        Protected Overridable Sub OnDocumentDownloadCompleteEx(ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            RaiseEvent DocumentDownloadCompleteEx(source)
        End Sub
        Protected Overridable Sub OnDocumentRetryStatusEx(ByVal currentTry As Integer, ByVal totalTries As Integer, ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            RaiseEvent DocumentRetryStatusEx(currentTry, totalTries, source)
        End Sub
        Protected Overridable Sub OnHeartbeatEx(ByVal msg As String, ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            If TradableInstrument IsNot Nothing Then
                RaiseEvent HeartbeatEx(String.Format("{0}:{1}", TradableInstrument.InstrumentIdentifier, msg), source)
            Else
                RaiseEvent HeartbeatEx(String.Format("{0}:{1}", "No instrument", msg), source)
            End If
        End Sub
        Protected Overridable Sub OnWaitingForEx(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String, ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            If TradableInstrument IsNot Nothing Then
                RaiseEvent WaitingForEx(elapsedSecs, totalSecs, String.Format("{0}-{1}", TradableInstrument.InstrumentIdentifier, msg), source)
            Else
                RaiseEvent WaitingForEx(elapsedSecs, totalSecs, String.Format("{0}-{1}", "No instrument", msg), source)
            End If
        End Sub
        Protected Overridable Sub OnDocumentDownloadComplete()
            RaiseEvent DocumentDownloadCompleteEx(New List(Of Object) From {Me})
        End Sub
        Protected Overridable Sub OnDocumentRetryStatus(ByVal currentTry As Integer, ByVal totalTries As Integer)
            RaiseEvent DocumentRetryStatusEx(currentTry, totalTries, New List(Of Object) From {Me})
        End Sub
        Protected Overridable Sub OnHeartbeat(ByVal msg As String)
            If TradableInstrument IsNot Nothing Then
                RaiseEvent HeartbeatEx(String.Format("{0}:{1}", TradableInstrument.InstrumentIdentifier, msg), New List(Of Object) From {Me})
            Else
                RaiseEvent HeartbeatEx(String.Format("{0}:{1}", "No instrument", msg), New List(Of Object) From {Me})
            End If
        End Sub
        Protected Overridable Sub OnWaitingFor(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String)
            If TradableInstrument IsNot Nothing Then
                RaiseEvent WaitingForEx(elapsedSecs, totalSecs, String.Format("{0}-{1}", TradableInstrument.InstrumentIdentifier, msg), New List(Of Object) From {Me})
            Else
                RaiseEvent WaitingForEx(elapsedSecs, totalSecs, String.Format("{0}-{1}", "No instrument", msg), New List(Of Object) From {Me})
            End If
        End Sub
#End Region

#Region "Logging and Status Progress"
        Public Shared logger As Logger = LogManager.GetCurrentClassLogger
#End Region


        <System.ComponentModel.Browsable(False)>
        Public Property ParentStrategy As Strategy
        <System.ComponentModel.Browsable(False)>
        Public Property TradableInstrument As IInstrument

        Protected _cts As CancellationTokenSource
        Protected _LastTick As ITick
        Protected _APIAdapter As APIAdapter

        'UI Properties
        <Display(Name:="Symbol", Order:=0)>
        Public Overridable ReadOnly Property TradingSymbol As String
            Get
                If TradableInstrument IsNot Nothing Then
                    Return TradableInstrument.TradingSymbol
                Else
                    Return Nothing
                End If
            End Get
        End Property
        <Display(Name:="Tradable", Order:=1)>
        Public Overridable ReadOnly Property Tradabale As Boolean
            Get
                If _LastTick IsNot Nothing Then
                    Return _LastTick.Tradable
                Else
                    Return Nothing
                End If
            End Get
        End Property
        <Display(Name:="Open", Order:=2)>
        Public Overridable ReadOnly Property OpenPrice As Decimal
            Get
                If _LastTick IsNot Nothing Then
                    Return _LastTick.Open
                Else
                    Return Nothing
                End If
            End Get
        End Property
        <Display(Name:="High", Order:=3)>
        Public Overridable ReadOnly Property HighPrice As Decimal
            Get
                If _LastTick IsNot Nothing Then
                    Return _LastTick.High
                Else
                    Return Nothing
                End If
            End Get
        End Property
        <Display(Name:="Low", Order:=4)>
        Public Overridable ReadOnly Property LowPrice As Decimal
            Get
                If _LastTick IsNot Nothing Then
                    Return _LastTick.Low
                Else
                    Return Nothing
                End If
            End Get
        End Property
        <Display(Name:="Previous Close", Order:=5)>
        Public Overridable ReadOnly Property ClosePrice As Decimal
            Get
                If _LastTick IsNot Nothing Then
                    Return _LastTick.Close
                Else
                    Return Nothing
                End If
            End Get
        End Property
        <Display(Name:="Volume", Order:=6)>
        Public Overridable ReadOnly Property Volume As Long
            Get
                If _LastTick IsNot Nothing Then
                    Return _LastTick.Volume
                Else
                    Return Nothing
                End If
            End Get
        End Property
        <Display(Name:="Average Price", Order:=7)>
        Public Overridable ReadOnly Property AveragePrice As Long
            Get
                If _LastTick IsNot Nothing Then
                    Return _LastTick.AveragePrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        <Display(Name:="Last Price", Order:=8)>
        Public Overridable ReadOnly Property LastPrice As Decimal
            Get
                If _LastTick IsNot Nothing Then
                    Return _LastTick.LastPrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        <Display(Name:="Timestamp", Order:=9)>
        Public Overridable ReadOnly Property Timestamp As Date?
            Get
                If _LastTick IsNot Nothing Then
                    Return _LastTick.Timestamp
                Else
                    Return Nothing
                End If
            End Get
        End Property


        Public Sub New(ByVal associatedInstrument As IInstrument, ByVal associatedParentStrategy As Strategy, ByVal canceller As CancellationTokenSource)
            TradableInstrument = associatedInstrument
            Me.ParentStrategy = associatedParentStrategy
            _cts = canceller
        End Sub
        Public MustOverride Overrides Function ToString() As String
        Public MustOverride Async Function RunDirectAsync() As Task
        Public MustOverride Async Function IsTriggerReachedAsync() As Task(Of Tuple(Of Boolean, Trigger))
        Public Overridable Async Function ProcessTickAsync(ByVal tickData As ITick) As Task
            Await Task.Delay(0).ConfigureAwait(False)
            NotifyPropertyChanged("Tradable")
            NotifyPropertyChanged("OpenPrice")
            NotifyPropertyChanged("HighPrice")
            NotifyPropertyChanged("LowPrice")
            NotifyPropertyChanged("ClosePrice")
            NotifyPropertyChanged("Volume")
            NotifyPropertyChanged("AveragePrice")
            NotifyPropertyChanged("LastPrice")
            NotifyPropertyChanged("Timestamp")
        End Function
    End Class
End Namespace
