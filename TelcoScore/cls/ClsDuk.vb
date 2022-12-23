Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Public Class ClsDuk

    Dim sErrDesc As String
#Region "Data Transaction"
    Public Function InsertRawresponse(ByVal sNIK As String, ByVal sRawResponse As String, ByVal UserCrt As String) As String
        'ReceiptNo, UserCrt, urlPDF
        Dim oDs As String
        Dim oData As New ClsData
        Dim sCmd As String
        'Dim oComm As New clsCommon

        sCmd = " exec spInsertRawresponse '" & sNIK & "'  ,  '" & sRawResponse & "'  , '" & UserCrt & "'  "

        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.oneColumn)

        Return oDs

    End Function
    Public Function UpdateRawresponse(ByVal sNIK As String, ByVal sRawResponse As String, ByVal UserCrt As String, ByVal sRequestId As String) As String
        'ReceiptNo, UserCrt, urlPDF
        Dim oDs As String
        Dim oData As New ClsData
        Dim sCmd As String
        'Dim oComm As New clsCommon

        sCmd = " exec spUpdateRawresponse '" & sNIK & "'  ,  '" & sRawResponse & "'  , '" & UserCrt & "'  , '" & sRequestId & "' "

        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.oneColumn)

        Return oDs

    End Function
    Public Function InsertReceiptDetailNIK(ByVal sData As String, ByVal sUserId As String, ByVal sImei As String) As String
        'ReceiptNo, UserCrt, urlPDF 
        Dim oDs As String
        Dim oData As New ClsData
        Dim sCmd As String
        'Dim oComm As New clsCommon

        sCmd = " exec SpInsertReceiptDetailNIK '" & sData & "', '" & sUserId & " ', '" & sImei & " ' "

        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.oneColumn)

        Return oDs

    End Function
    Public Function InsertRequestApprovalChangePassword(ByVal sData As String) As String
        'ReceiptNo, UserCrt, urlPDF 
        Dim oDs As String
        Dim oData As New ClsData
        Dim sCmd As String
        'Dim oComm As New clsCommon

        sCmd = " exec spInsertRequestApprovalChangePassword '" & sData & "'    "


        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.oneColumn)

        Return oDs

    End Function
    Public Function SendRequestApprovalChangePassword(ByVal sData As String) As String
        'ReceiptNo, UserCrt, urlPDF 
        Dim oDs As String
        Dim oData As New ClsData
        Dim sCmd As String
        'Dim oComm As New clsCommon

        sCmd = " exec SpSendRequestApprovalChangePassword '" & sData & "'    "
        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.oneColumn)

        Return oDs

    End Function

    Public Function CheckRegisteredUser(ByVal sImei As String, ByVal sUserId As String) As String
        'ReceiptNo, UserCrt, urlPDF 
        Dim oDs As String
        Dim oData As New ClsData
        Dim sCmd As String
        'Dim oComm As New clsCommon

        sCmd = " exec spCheckRegisteredUser '" & sImei & "' , '" & sUserId & "'    "


        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.oneColumn)

        Return oDs

    End Function

    Public Function CheckValidateUser(ByVal sData As String) As String
        'ReceiptNo, UserCrt, urlPDF 
        Dim oDs As String
        Dim oData As New ClsData
        Dim sCmd As String
        'Dim oComm As New clsCommon

        sCmd = " exec spCheckValidateUser '" & sData & "'    "


        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.oneColumn)

        Return oDs

    End Function
    Public Function CheckRequestApprovalChangePassword(ByVal sData As String) As String
        'ReceiptNo, UserCrt, urlPDF 
        Dim oDs As String
        Dim oData As New ClsData
        Dim sCmd As String
        'Dim oComm As New clsCommon

        sCmd = " exec spCheckRequestApprovalChangePassword '" & sData & "'    "


        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.oneColumn)

        Return oDs

    End Function
    'insertDataBalikan(sNik, sNO_KONTRAK, sResult)
    Public Function insertDataBalikan(ByVal sNik As String, ByVal sNO_KONTRAK As String, ByVal sResult As String) As DataSet
        Dim oDs As Object
        Dim oData As New ClsData
        Dim sCmd As String
        Dim iErrNo = 0

        sCmd = "exec spinsertDataBalikan    '" & sNik & "' , '" & sNO_KONTRAK & "' , '" & sResult & "' "
        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.DataSet, iErrNo)
        Return oDs

    End Function


    Public Function GetAgreementnoByNIk(ByVal sNik As String) As DataSet
        Dim oDs As Object
        Dim oData As New ClsData
        Dim sCmd As String
        Dim iErrNo = 0

        sCmd = "exec spGetAgreementnoByNIk  '" & sNik & "'  "
        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.DataSet, iErrNo)
        Return oDs

    End Function

    Public Function GetSendFeedBackToDukcapil() As DataSet
        Dim oDs As Object
        Dim oData As New ClsData
        Dim sCmd As String
        Dim iErrNo = 0

        sCmd = "exec spGetSendFeedBackToDukcapil   "
        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.DataSet, iErrNo)
        Return oDs

    End Function


    Public Function GetListUnFeedBack(ByVal sNik As String) As DataSet
        Dim oDs As Object
        Dim oData As New ClsData
        Dim sCmd As String
        Dim iErrNo = 0

        sCmd = "exec spGetListUnFeedBack  '" & sNik & "'  "
        oDs = oData.executeQuery(sCmd, ClsData.ReturnType.DataSet, iErrNo)
        Return oDs

    End Function



#End Region

End Class
