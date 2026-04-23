VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CGreeting"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

Dim mType      As Integer
Dim mMessage   As String
Dim mFormat    As String
Dim mLngParam  As String


Public Property Get gType() As Integer
   gType = mType
End Property

Public Property Let gType(ByVal vNewValue As Integer)
   mType = vNewValue
End Property

Public Property Get message() As String
   message = mMessage
End Property

Public Property Let message(ByVal vNewValue As String)
   mMessage = vNewValue
End Property

Public Property Get format() As String
   format = mFormat
End Property

Public Property Let format(ByVal vNewValue As String)
   mFormat = vNewValue
End Property

Public Property Get LangParam() As String
   LangParam = mLngParam
End Property

Public Property Let LangParam(ByVal vNewValue As String)
   mLngParam = vNewValue
End Property
