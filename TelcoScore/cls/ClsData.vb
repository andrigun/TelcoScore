Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient

Public Class ClsData
    Public oConn As New SqlConnection       ' for sql connection

    ' 2 varaible ini diperlukan untuk execute dengan begin transaction
    Public oTransaction As SqlTransaction
    Public oCommand As SqlCommand

    Private _db_server As String = ConfigurationManager.AppSettings("ServerName")
    Private _db_name As String = ConfigurationManager.AppSettings("DatabaseName")
    Private _user_id As String = ConfigurationManager.AppSettings("UserID")
    Private _pwd As String = ConfigurationManager.AppSettings("Password")
    Public Enum ReturnType
        DataReader = 0
        DataSet = 1
        NonQueryExecute = 2
        Paging = 3
        oneColumn = 4
    End Enum

    Public Function openConnection() As Boolean
        On Error GoTo errHandle

        oConn.ConnectionString = "Server=" & _db_server & "; User ID=" & _user_id & "; Pwd=" & _pwd & "; Database=" & _db_name & ";Connection Timeout=320"
        oConn.Open()
        openConnection = True
        Exit Function

errHandle:
        If oConn.State = ConnectionState.Open Then
            oConn.Close()
            Resume Next
        Else
            openConnection = False
        End If
    End Function

    '    Public Function openConnection2() As Boolean
    '        On Error GoTo errHandle

    '        oConn.ConnectionString = "Server=" & _db_server2 & "; User ID=" & _user_id2 & "; Pwd=" & _pwd2 & "; Database=" & _db_name2
    '        oConn.Open()
    '        openConnection2 = True
    '        Exit Function

    'errHandle:
    '        If oConn.State = ConnectionState.Open Then
    '            oConn.Close()
    '            Resume Next
    '        Else
    '            openConnection2 = False
    '        End If
    '    End Function

    Public Function getConnection(ByVal dbServer As String, ByVal userID As String, ByVal password As String, ByVal dbName As String) As SqlClient.SqlConnection
        On Error GoTo errHandle

        oConn.ConnectionString = "Server=" & dbServer & "; User ID=" & userID & "; Pwd=" & password & "; Database=" & dbName
        oConn.Open()
        Return oConn
        Exit Function

errHandle:
        Err.Raise(1000, , "Error Create Connection")

    End Function


    Public Sub closeConnection()
        oConn.Close()
    End Sub
    ''CLASS EQ1
    ''UNTUK CLASS YANG ADA QUERYNYA
    Public Function executeQuery(ByVal sQuery As String, ByVal nReadMode As ReturnType, Optional ByRef iErrNo As Long = 0) As Object
        On Error GoTo errHandle
        Select Case nReadMode
            Case ReturnType.DataReader
                ' for execute and get the datareader
                Dim oCmd As New SqlCommand(sQuery, oConn)
                Dim oDataReader As SqlDataReader
                If openConnection() = True Then
                    oDataReader = oCmd.ExecuteReader()
                    Return oDataReader
                Else
                    Return "Problem in Database Connection"
                End If
            Case ReturnType.DataSet
                Dim oAdapter As New SqlDataAdapter(sQuery, oConn)
                Dim adapter As SqlDataAdapter = New SqlDataAdapter()

                Dim oDs As New DataSet
                If openConnection() = True Then
                    Dim cmd As SqlCommand = New SqlCommand(sQuery)

                    cmd.CommandTimeout = 900 ' Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings("SqlCommandTimeOut"))

                    adapter.SelectCommand = New SqlCommand(sQuery, oConn)
                    adapter.SelectCommand.CommandTimeout = 900
                    adapter.Fill(oDs)

                    '  oAdapter.Fill(oDs, "DataSet")

                    closeConnection()
                    executeQuery = oDs
                    'Return oDs
                Else
                    Return "Problem in Database Connection"
                End If
            Case ReturnType.NonQueryExecute
                Dim oCmd As New SqlCommand(sQuery, oConn)
                Dim nAffected As Integer
                If openConnection() = True Then
                    nAffected = oCmd.ExecuteNonQuery()
                    closeConnection()
                    Return nAffected
                Else
                    Return "Problem in Database Connection"
                End If
            Case ReturnType.oneColumn
                Dim oCmd As New SqlCommand(sQuery, oConn)
                Dim oColumnReturn As Object
                If openConnection() = True Then
                    oCmd.CommandTimeout = 320
                    oColumnReturn = oCmd.ExecuteScalar()
                    closeConnection()
                    Return oColumnReturn
                Else
                    Return "Problem in Database Connection"
                End If
            Case ReturnType.Paging
        End Select
        Exit Function

errHandle:
        iErrNo = Err.Number
        Return Err.Description
    End Function
    ''CLASS EQ2
    ''UNTUK CLASS YANG ADA SqlClient.SqlConnection
    Public Function executeQuery(ByVal par_oConn As SqlClient.SqlConnection, ByVal sQuery As String, ByVal nReadMode As ReturnType, Optional ByRef sErrNo As Long = 0) As Object
        On Error GoTo errHandle
        Select Case nReadMode
            Case ReturnType.DataReader
                ' for execute and get the datareader
                Dim oCmd As New SqlCommand(sQuery, par_oConn)
                Dim oDataReader As SqlDataReader
                oDataReader = oCmd.ExecuteReader()
                Return oDataReader

            Case ReturnType.DataSet
                Dim oAdapter As New SqlDataAdapter(sQuery, par_oConn)
                Dim oDs As New DataSet

                oAdapter.Fill(oDs, "DataSet")
                closeConnection()
                executeQuery = oDs
                'Return oDs

            Case ReturnType.NonQueryExecute
                Dim oCmd As New SqlCommand(sQuery, par_oConn)
                Dim nAffected As Integer
                nAffected = oCmd.ExecuteNonQuery()
                closeConnection()
                Return nAffected

            Case ReturnType.oneColumn
                Dim oCmd As New SqlCommand(sQuery, par_oConn)
                Dim oColumnReturn As Object
                oColumnReturn = oCmd.ExecuteScalar()
                closeConnection()
                Return oColumnReturn

            Case ReturnType.Paging
        End Select
        Exit Function

errHandle:
        sErrNo = Err.Number
        Return Err.Description
    End Function
    ''CLASS EQ3
    ''UNTUK CLASS YANG ADA QUERYNYA (pAGING)
    Public Function ExecuteQuery(ByVal szQuery As String, ByVal nReadMode As ReturnType, ByVal iCurrPage As Integer, ByVal iRowPerPage As Integer, ByRef iTotalPage As Integer) As Object
        On Error GoTo Err
        Select Case nReadMode
            Case ReturnType.Paging
                Dim oDataAdapter As New SqlDataAdapter(szQuery, oConn)
                Dim oDs As New DataSet
                Dim iTotalRow As Integer
                Dim iCurrIdx As Integer

                iCurrIdx = (iCurrPage * iRowPerPage) - iRowPerPage

                If openConnection() Then
                    ' get total page
                    oDataAdapter.Fill(oDs, "DataSet")
                    iTotalRow = oDs.Tables(0).Rows.Count

                    iTotalPage = CInt(Math.Ceiling(CDbl(iTotalRow) / iRowPerPage))
                    ' end of get total page

                    ' fill with real record
                    oDs = New DataSet
                    oDataAdapter.Fill(oDs, iCurrIdx, iRowPerPage, "DataSet")

                    closeConnection()
                    ExecuteQuery = oDs
                    'Return objDataSet
                Else
                    Return "Problem in Database Connetion"
                End If

        End Select
        Exit Function
Err:
        Return Err.Description
    End Function

    Public Function beginTrans() As Object
        If oConn.State <> ConnectionState.Open Then
            If openConnection() = False Then
                Return "Problem in Database Connection"
            End If
        End If
        oTransaction = oConn.BeginTransaction
        oCommand = oConn.CreateCommand
        oCommand.Transaction = oTransaction
    End Function

    Public Function commitTrans() As Object
        oTransaction.Commit()
    End Function


    Public Function rollbackTrans() As Object
        oTransaction.Rollback()
    End Function


    Public Function ConnectGridView(ByVal SqlSource As SqlDataSource, ByVal Query As String) As Boolean
        SqlSource.ConnectionString = "Server=" & _db_server & "; User ID=" & _user_id & "; Pwd=" & _pwd & "; Database=" & _db_name
        SqlSource.SelectCommand = Query
    End Function

End Class
