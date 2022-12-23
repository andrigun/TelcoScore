Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient

Public Class ClsPay

    Dim sErrDesc As String = ""

    Public Function GetCurrentToken() As String
        Dim oDs As String
        Dim oData As New ClsData
        Dim sCmd As String
        Dim iErrNo = 0

        sCmd = "exec SpGetCurrentToken   "
        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.oneColumn)

        Return oDs

    End Function
    Public Function GetCurrentTokenByProvider(ByVal ProviderId As String) As String
        Dim oDs As String
        Dim oData As New ClsData
        Dim sCmd As String
        Dim iErrNo = 0

        sCmd = "exec spGetCurrentTokenByProvider '" & ProviderId & "'"
        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.oneColumn)

        Return oDs

    End Function
    Public Function LoadDetailApplicationNo(ByVal ApplicationNo As String) As DataSet
        Dim oDs As Object
        Dim oData As New ClsData
        Dim sCmd As String
        Dim iErrNo = 0

        'sCmd = "exec spLoadDetailApplicationNo_TEST  '" & Trim(ApplicationNo) & "'    "  ' keperluan test 20210821
        sCmd = "exec spLoadDetailApplicationNo  '" & Trim(ApplicationNo) & "'    "

        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.DataSet, iErrNo)
        Return oDs


    End Function
    Public Function LoadScoreByMobilephone(ByVal sMobilephone As String) As DataSet
        Dim oDs As Object
        Dim oData As New ClsData
        Dim sCmd As String
        Dim iErrNo = 0

        'sCmd = "exec spLoadDetailApplicationNo_TEST  '" & Trim(ApplicationNo) & "'    "  ' keperluan test 20210821
        sCmd = "exec spLoadScoreByMobilephone  '" & Trim(sMobilephone) & "'    "

        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.DataSet, iErrNo)
        Return oDs


    End Function


    Public Function GetDetailProvider(ByVal sMobilePhone As String) As DataSet
        Dim oDs As Object
        Dim oData As New ClsData
        Dim sCmd As String
        Dim iErrNo = 0

        sCmd = "exec spGetDetailProvider  '" & Trim(sMobilePhone) & "'    "
        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.DataSet, iErrNo)
        Return oDs
    End Function

    Public Function InsertCurrentTokenByProviderID(ByVal sToken As String, ByVal ProviderId As String) As String

        Dim oDs As String
        Dim oData As New ClsData
        Dim sCmd As String
        'Dim oComm As New clsCommon

        sCmd = " exec SpInsertCurrentTokenByProviderID '" & sToken & "'  , '" & ProviderId & "'"

        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.oneColumn)

        Return oDs

    End Function

    Public Function InsertCurrentToken(ByVal sToken As String) As String

        Dim oDs As String
        Dim oData As New ClsData
        Dim sCmd As String
        'Dim oComm As New clsCommon

        sCmd = " exec SpInsertCurrentToken '" & sToken & "'   "

        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.oneColumn)

        Return oDs

    End Function


    Public Function InsertRequestLog(ByVal RequestId As String, ByVal MobilePhone As String,
                                     ByVal ResponseMsg As String, ByVal UserID As String) As String

        Dim oDs As String
        Dim oData As New ClsData
        Dim sCmd As String
        Dim oComm As New clsCommon

        sCmd = " exec SpInsertRequestLog '" & RequestId & "' , '" & MobilePhone & "' , '" & oComm.DesKutip(ResponseMsg) & "'  , '" & UserID & "'  "

        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.oneColumn)

        Return oDs

    End Function

    Public Function InsertResponseLog(ByVal RequestId As String, ByVal MobilePhone As String,
                                     ByVal UserID As String, ByVal ResponseMsg As String) As String

        'oDs = oLoad.InsertResponseLog(ApplicationNo, MobilePhone, usrReq, jsonData)

        Dim oDs As String
        Dim oData As New ClsData
        Dim sCmd As String
        Dim oComm As New clsCommon

        sCmd = " exec SpInsertResponseLog'" & RequestId & "' , '" & MobilePhone & "' , '" & UserID & "'  , '" & oComm.DesKutip(ResponseMsg) & "'  "

        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.oneColumn)

        Return oDs

    End Function





    Public Function GetListPaymentArDept(ByVal sFrom As String, ByVal sTo As String, ByVal sStatus As String, ByVal sPost As String) As DataSet
        Dim oDs As Object
        Dim oData As New ClsData
        Dim sCmd As String
        Dim iErrNo = 0

        sCmd = "exec SpGetListPaymentArDept  '" & Trim(sFrom) & "'  , '" & Trim(sTo) & "'   , '" & sStatus & "'  , '" & sPost & "'  "
        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.DataSet, iErrNo)
        Return oDs

    End Function

End Class
