
Imports TelcoScore.ClsData

Public Class clsUser

    Inherits System.Web.UI.Page           ' supaya object dari web page , seperti session bisa dikenal.

    Private _db_server As String = ConfigurationManager.AppSettings("AAMServerName")
    Private _db_name As String = ConfigurationManager.AppSettings("AAMDatabaseName")
    Private _user_id As String = ConfigurationManager.AppSettings("AAMUserID")
    Private _pwd As String = ConfigurationManager.AppSettings("AAMPassword")

#Region "Permission Right"


    Public Function getApplicationList(ByVal userID As String, ByVal isAll As Boolean, Optional ByRef errDesc As String = "") As DataSet
        ' fungsi ini digunakan untuk mendapatkan informasi dari User yang umum dipakai untk pengecekan ,

        Dim oData As New clsData
        Dim sCmd As String
        Dim vReturn

        If isAll Then
            sCmd = "exec getapplicationList '" & userID & "'"
        Else
            sCmd = "exec getapplicationListByUser '" & userID & "'"
        End If

        Return oData.executeQuery(sCmd, clsData.ReturnType.DataSet)

        oData = Nothing

    End Function


    Public Function getUserNameIfValid(ByVal userID As String, ByVal userEncrypted As String, Optional ByRef msg As String = "") As String

        Dim oData As New clsData
        Dim sCmd As String
        sCmd = "exec getUserNameIfValid '" & userID & "', '" & userEncrypted & "'"
        Return oData.executeQuery(oData.getConnection(_db_server, _user_id, _pwd, _db_name), sCmd, ReturnType.oneColumn)

    End Function

    Public Function getApplicationAccessRight(ByVal userID As String, ByVal appID As String, Optional ByRef msg As String = "") As Boolean
        Dim oData As New clsData
        Dim isAccess As Boolean
        Dim sCmd As String

        sCmd = "exec getApplicationAccessRight '" & userID & "', '" & appID & "'"

        isAccess = oData.executeQuery(oData.getConnection(_db_server, _user_id, _pwd, _db_name), sCmd, ReturnType.oneColumn)
        Return isAccess


    End Function



    Public Function getModuleAccessRight(ByVal userID As String, ByVal appID As String, ByVal moduleID As String, Optional ByRef msg As String = "") As Boolean
        Dim oData As New clsData
        Dim isAccess As Boolean
        Dim sCmd As String

        sCmd = "exec getModuleAccessRight '" & userID & "', '" & appID & "', '" & moduleID & "'"

        isAccess = oData.executeQuery(oData.getConnection(_db_server, _user_id, _pwd, _db_name), sCmd, ReturnType.oneColumn)
        Return isAccess

    End Function


    Public Function getModuleFunctionAccessRight(ByVal userID As String, ByVal appID As String, ByVal moduleID As String, ByVal functionID As String, Optional ByRef msg As String = "") As Boolean
        Dim oData As New clsData
        Dim isAccess As Boolean
        Dim sCmd As String

        sCmd = "exec getModuleFunctionAccessRight '" & userID & "', '" & appID & "', '" & moduleID & "', '" & functionID & "'"

        isAccess = oData.executeQuery(oData.getConnection(_db_server, _user_id, _pwd, _db_name), sCmd, ReturnType.oneColumn)
        Return isAccess

    End Function


    Public Function getIsGroup(ByVal userID As String, ByVal groupID As String, Optional ByRef msg As String = "") As Boolean
        Dim oData As New clsData
        Dim sCmd As String
        Dim sGroupID As String

        sCmd = "exec getIsGroup '" & userID & "', '" & groupID & "'"

        sGroupID = oData.executeQuery(oData.getConnection(_db_server, _user_id, _pwd, _db_name), sCmd, ReturnType.oneColumn)
        If sGroupID = "" Or IsDBNull(sGroupID) Then
            Return False
        Else
            Return True
        End If

    End Function

    Public Function getUserInfo(ByVal userID As String, Optional ByRef errDesc As String = "") As DataSet
        ' fungsi ini digunakan untuk mendapatkan informasi dari User yang umum dipakai untk pengecekan ,

        Dim oData As New clsData
        Dim sCmd As String
        Dim vReturn

        sCmd = "select userName, email, jobPositionID from employee where userID = '" & userID & "'"
        Return oData.executeQuery(sCmd, clsData.ReturnType.DataSet)

        oData = Nothing

    End Function


    Public Function getUserData(ByVal userID As String, Optional ByRef msg As String = "") As DataSet
        Dim oData As New clsData
        Dim sCmd As String

        sCmd = "exec getUserData '" & userID & "'"

        Return oData.executeQuery(oData.getConnection(_db_server, _user_id, _pwd, _db_name), sCmd, ReturnType.DataSet)

    End Function


    Public Function getUserID(Optional ByRef Logout As String = "") As String
        'Return "dhoni.oktafiansyah"
        'Return "sean.lily"
        'Return "nazori"
        'Return "nani.susanti"
        'Return "donny.triwardono"
        'Return "sundoyo"
        'Return "eko.budianto"
        'Return "ronald.sinaga"
        If Logout = "LOGOUT" Then
            Session.Abandon()
        Else
            Return Session("userLogin")
            'Return "amir.hamzah"
        End If

        'Return "akhmad.sardani"

    End Function

#End Region


#Region "user information"


    Public Function isUserInformationExists(ByVal userID As String, Optional ByRef msg As String = "") As Boolean
        Dim oData As New clsData
        Dim sCmd As String
        Dim sUserID As String

        sCmd = "select userID from userInformation where userID='" & userID & "'"
        sUserID = oData.executeQuery(oData.getConnection(_db_server, _user_id, _pwd, _db_name), sCmd, ReturnType.oneColumn)
        If sUserID = "" Or IsDBNull(sUserID) Then
            Return False
        Else
            Return True
        End If

    End Function


    Public Function getUserInformation(ByVal userID As String, Optional ByRef msg As String = "") As DataSet

        Dim oData As New clsData
        Dim sCmd As String
        sCmd = "exec user_getInformation '" & userID & "'"
        Return oData.executeQuery(oData.getConnection(_db_server, _user_id, _pwd, _db_name), sCmd, ReturnType.DataSet)

    End Function


    Public Function userAddInformation(ByVal userID As String, ByVal branchID As String, ByVal divisionID As String, ByVal departmentID As String, ByVal jobPositionID As String, ByVal email As String, ByVal usrUpd As String, Optional ByRef errDesc As String = "") As Boolean

        Dim oData As New clsData
        Dim oComm As New clsCommon
        Dim iErrNo As Long

        ' open connection , harus karena tidak menggunakan method execute yg ada di clsData
        If Not oData.openConnection Then
            Return False
            errDesc = "Database Connection Failed."
        End If


        oData.beginTrans()
        Try

            ' adding parameter
            oData.oCommand.Parameters.Clear()
            Dim oParam As New SqlClient.SqlParameter
            oParam = oData.oCommand.Parameters.Add("@userID", SqlDbType.VarChar, 50)
            oParam.Value = userID

            oParam = oData.oCommand.Parameters.Add("@branchID", SqlDbType.Char, 3)
            oParam.Value = branchID

            oParam = oData.oCommand.Parameters.Add("@divisionID", SqlDbType.Char, 3)
            oParam.Value = divisionID

            oParam = oData.oCommand.Parameters.Add("@departmentID", SqlDbType.Char, 3)
            oParam.Value = departmentID

            oParam = oData.oCommand.Parameters.Add("@jobPositionID", SqlDbType.Char, 3)
            oParam.Value = jobPositionID

            oParam = oData.oCommand.Parameters.Add("@email", SqlDbType.Char, 3)
            oParam.Value = email

            oParam = oData.oCommand.Parameters.Add("@usrUpd", SqlDbType.VarChar, 50)
            oParam.Value = usrUpd

            oData.oCommand.CommandType = CommandType.StoredProcedure
            oData.oCommand.CommandText = "user_AddInformation"
            oData.oCommand.ExecuteNonQuery()


            oData.commitTrans()
            oData.closeConnection()

            Return True


        Catch ex As Exception
            errDesc = "Source : " & ex.Source & " , Desc : " & ex.Message
            oData.rollbackTrans()
            oData.closeConnection()
            Return False
        End Try
    End Function


    Public Function userEditInformation(ByVal userID As String, ByVal branchID As String, ByVal divisionID As String, ByVal departmentID As String, ByVal jobPositionID As String, ByVal email As String, ByVal usrUpd As String, ByRef errDesc As String) As Boolean

        Dim oData As New clsData
        Dim oComm As New clsCommon
        Dim iErrNo As Long

        ' open connection , harus karena tidak menggunakan method execute yg ada di clsData
        If Not oData.openConnection Then
            Return False
            errDesc = "Database Connection Failed."
        End If


        oData.beginTrans()
        Try

            ' adding parameter
            oData.oCommand.Parameters.Clear()
            Dim oParam As New SqlClient.SqlParameter
            oParam = oData.oCommand.Parameters.Add("@userID", SqlDbType.VarChar, 50)
            oParam.Value = userID

            oParam = oData.oCommand.Parameters.Add("@branchID", SqlDbType.Char, 3)
            oParam.Value = branchID

            oParam = oData.oCommand.Parameters.Add("@divisionID", SqlDbType.Char, 3)
            oParam.Value = divisionID

            oParam = oData.oCommand.Parameters.Add("@departmentID", SqlDbType.Char, 3)
            oParam.Value = departmentID

            oParam = oData.oCommand.Parameters.Add("@jobPositionID", SqlDbType.Char, 3)
            oParam.Value = jobPositionID

            oParam = oData.oCommand.Parameters.Add("@email", SqlDbType.Char, 3)
            oParam.Value = email

            oParam = oData.oCommand.Parameters.Add("@usrUpd", SqlDbType.VarChar, 50)
            oParam.Value = usrUpd

            oData.oCommand.CommandType = CommandType.StoredProcedure
            oData.oCommand.CommandText = "user_EditInformation"
            oData.oCommand.ExecuteNonQuery()


            oData.commitTrans()
            oData.closeConnection()

            Return True


        Catch ex As Exception
            errDesc = "Source : " & ex.Source & " , Desc : " & ex.Message
            oData.rollbackTrans()
            oData.closeConnection()
            Return False
        End Try
    End Function

#End Region

#Region "Search"

    Public Function searchTaskForReview(ByVal userID As String, Optional ByRef errDesc As String = "") As DataSet
        ' fungsi ini digunakan untuk mencari Request - Request yg perlu di review oleh user 
        ' (member RequestReviewer) di halaman Home

        Dim oData As New clsData
        Dim sCmd As String
        Dim vReturn

        sCmd = "exec taskForReview '" & userID & "'"
        Return oData.executeQuery(sCmd, clsData.ReturnType.DataSet)

        oData = Nothing

    End Function


    Public Function searchTaskForReviewResponse(ByVal userID As String, Optional ByRef errDesc As String = "") As DataSet
        ' fungsi ini digunakan untuk mencari Request - Request yg perlu di review oleh user 
        ' (member RequestReviewer) di halaman Home

        Dim oData As New clsData
        Dim sCmd As String
        Dim vReturn

        sCmd = "exec taskForReviewResponse '" & userID & "'"
        Return oData.executeQuery(sCmd, clsData.ReturnType.DataSet)

        oData = Nothing

    End Function


    Public Function searchTaskForApproval(ByVal userID As String, Optional ByRef errDesc As String = "") As DataSet
        ' fungsi ini digunakan untuk mencari Request - Request yg perlu di review oleh user 
        ' (member RequestReviewer) di halaman Home

        Dim oData As New clsData
        Dim sCmd As String
        Dim vReturn

        sCmd = "exec taskForApproval '" & userID & "'"
        Return oData.executeQuery(sCmd, clsData.ReturnType.DataSet)

        oData = Nothing

    End Function

#End Region
    Public Function getUserApprovalList(ByVal currPage As Integer, ByVal rowPerPage As Integer, ByRef totalPage As Integer, ByVal userID As String, ByVal appID As String, ByVal appApprovalID As String) As DataSet
        ' by vw , 16 Jul 2008
        ' SP related : 

        Dim oDs As DataSet
        Dim oData As New clsData
        Dim sCmd As String


        sCmd = "exec apr_getUserApprovalList '" & userID & "', '" & appID & "', '" & appApprovalID & "'"

        oDs = oData.executeQuery(oData.getConnection(_db_server, _user_id, _pwd, _db_name), sCmd, ReturnType.DataSet)
        'oDs = oData.executeQuery(oData.getConnection(_db_server, _user_id, _pwd, _db_name), sCmd, clsData.ReturnType.Paging, currPage, rowPerPage, totalPage)
        ' oDs = oData.executeQuery(oData.getConnection(_db_server, _user_id, _pwd, _db_name), sCmd, clsData.ReturnType.oneColumn)

        Return oDs

    End Function
   

End Class
