VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CParameter"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Attribute VB_Ext_KEY = "SavedWithClassBuilder" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"Yes"
Option Explicit

Dim mName As String
Dim mValue As String
Dim mDefault As String
Dim mDescr As String

Public Property Get name() As String
   name = mName
End Property

Public Property Let name(ByVal vNewValue As String)
   mName = vNewValue
End Property

Public Property Get value() As String
   value = mValue
End Property

Public Property Let value(ByVal vNewValue As String)
   mValue = vNewValue
End Property

Public Property Get default() As String
   default = mDefault
End Property

Public Property Let default(ByVal vNewValue As String)
   mDefault = vNewValue
End Property

Public Property Get description() As String
   description = mDescr
End Property

Public Property Let description(ByVal vNewValue As String)
   mDescr = vNewValue
End Property

Public Function paramString() As String
   paramString = Chr(254) + name + Chr(255) + _
                 default + Chr(255) + _
                 description + Chr(255) + _
                 value + Chr(255)
End Function

