
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.WebService
Imports System.Web.Services.Protocols
Imports System.Web.Script.Services.ScriptServiceAttribute
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services
Imports System.Net
Imports System.IO
Imports System.Web.UI.Page
Imports System.Web.Http

Imports System.Web.HttpContext

Imports System.Net.WebClient

Imports System.ComponentModel
Imports System.Net.HttpRequestHeader

Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json.Schema
Imports Microsoft.VisualBasic

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Runtime.Serialization.Json
Imports System.Collections.Generic
Imports System.Linq
Imports System.Configuration
Imports System.ServiceModel.Web
Imports System.Text

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.AttributeUsage(System.AttributeTargets.[Class] Or System.AttributeTargets.[Interface])>
<ToolboxItem(False)> _
Public Class WsGetScore
    Inherits System.Web.Services.WebService

    Private _UrlEncrypted As String = ConfigurationManager.AppSettings("sUrlEncrypted")
    Private _UrlDecrypted As String = ConfigurationManager.AppSettings("sUrlDecrypted")

    Private _UrlToken As String = ConfigurationManager.AppSettings("sUrlToken")
    Private _UrlGetScore As String = ConfigurationManager.AppSettings("sUrlGetScore")

    Private _UrlGetScoreInternal As String = ConfigurationManager.AppSettings("sUrlGetScoreInternal")
    Private _sUserRequest As String = ConfigurationManager.AppSettings("sUserRequest")


    Dim sToken As String = ""
    Dim sEncrypted As String = ""
    Dim sEncryptedScore As String = ""
    Dim sRequestID As String = ""
    Dim MobilePhone As String = ""

    Dim sUserID As String = ""
    <WebMethod(),
    ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json, XmlSerializeString:=False)>
    Public Function FnGetScoreByMobilePhoneInternal(ByVal sMobilePhone As String,
                                                    ByVal RequestID As String, ByVal UserID As String) As String
        Try
            Dim Httprequest As HttpWebRequest
            Dim response As HttpWebResponse
            Dim reader As StreamReader
            Dim rawresponse As String = ""

            Dim sResult As String = ""
            sUserID = UserID

            Dim urlWebsrvices As String
            urlWebsrvices = _UrlGetScoreInternal '"http://10.10.1.48/TelcoScore/WsGetScore.asmx/FnGetScoreByMobilePhone"

            ServicePointManager.Expect100Continue = True
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls ' SecurityProtocolType.Tls12
            ServicePointManager.SecurityProtocol = DirectCast(3072, SecurityProtocolType)

            Httprequest = DirectCast(WebRequest.Create(urlWebsrvices), HttpWebRequest)
            Httprequest.ContentType = "application/json"
            Httprequest.Method = "POST"
            'request.TransferEncoding = True
            Httprequest.AllowAutoRedirect = True
            Httprequest.Proxy.Credentials = CredentialCache.DefaultCredentials
            Httprequest.ContentType = "application/json"
            Httprequest.UseDefaultCredentials = True
            Httprequest.Accept = "application/json"
            Httprequest.KeepAlive = True
            Httprequest.ProtocolVersion = HttpVersion.Version10

            'Request.ProtocolVersion = HttpVersion.Version10;

            Dim requestd As New MyRequestData With {
                        .sMobilePhone = sMobilePhone,
                        .RequestID = RequestID,
                        .UserID = UserID
                        }

            Dim postdata As String = JsonConvert.SerializeObject(requestd)
            Dim requestWriter As StreamWriter = New StreamWriter(Httprequest.GetRequestStream())
            requestWriter.Write(postdata)
            requestWriter.Close()

            response = DirectCast(Httprequest.GetResponse(), HttpWebResponse)
            reader = New StreamReader(response.GetResponseStream())
            rawresponse = reader.ReadToEnd()
            Dim sTable As String = ""

            Dim ser As JObject = JObject.Parse(rawresponse)
            Dim data As List(Of JToken) = ser.Children().ToList
            For Each item As JProperty In data
                item.CreateReader()
                Select Case item.Name
                    Case "response"
                        ' sResult = item.Value
                        sTable = "<TABLE cellSpacing='0' cellPadding='0' width='95%' border='0' id='Table' >"
                        sTable = sTable + "<TR class='trtopi'>" &
                                "<TD class='tdtopikiri' width='10' height='20'>&nbsp;</TD>" &
                                      "<TD class='tdtopi' align='center'> Telco Score</TD>" &
                                      "<TD class='tdtopikanan' width='10'>&nbsp;</TD>" &
                                     "</TR> </TABLE>"

                        sTable = sTable & "<table  class='tablegrid' cellSpacing='1' cellPadding='2'  border='1' width='95%'  >"
                        sTable = sTable + "<tr> <td style='padding-left: 5px; padding-bottom: 3px; align:center'>  <font color='red'> <strong>" & item.Value.ToString
                        sTable = sTable + "</strong> </font> </td></tr> </table>"
                        sResult = sTable
                End Select
            Next



            Return sResult

        Catch ex As Exception
            Return ex.Message
        End Try

    End Function

    <WebMethod(),
    ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json, XmlSerializeString:=False)>
    Public Function FnGetScoreByMobilePhone1(ByVal sMobilePhone As String,
                                            ByVal RequestID As String, ByVal UserID As String) As String
        Dim sb As New StringBuilder
        Dim sw As New IO.StringWriter(sb)
        Dim strOut As String = String.Empty

        Dim Httprequest As HttpWebRequest
        Dim response As HttpWebResponse
        Dim reader As StreamReader
        Dim rawresponse As String = ""

        Dim sResult As String = ""
        Dim rScore As String = ""

        sRequestID = Trim(RequestID.ToString)
        Try

            'Get Token 
            sToken = getToken() ' get from database

            'Get Encrypted MSISDN
            sEncrypted = getEncrypted(sMobilePhone) ' validasi number

            'Request Score
            sEncryptedScore = sendRequestScore(Trim(sEncrypted.ToString), Trim(RequestID.ToString), Trim(sMobilePhone.ToString), UserID)
            If Len(sEncryptedScore) < 100 Then
                sResult = sEncryptedScore
            Else
                'Decrypt Score 
                sResult = decryptScore(sEncryptedScore)
            End If

            Dim oDs As String ' New DataSet
            Dim oLoad As New ClsPay
            oDs = oLoad.InsertRequestLog(RequestID, sMobilePhone, sResult, UserID)


            Using jr As New JsonTextWriter(sw)
                With jr
                    '.CloseOutput = False
                    .WriteStartObject()
                    .WritePropertyName("response")
                    .WriteValue(sResult)
                    .WriteEndObject()
                    '.WriteEnd()
                    '.CloseOutput = False
                End With
                strOut = sw.ToString
            End Using

            HttpContext.Current.Response.Clear()
            HttpContext.Current.Response.ContentType = "application/json; charset=utf-8"
            HttpContext.Current.Response.Write(strOut)
            HttpContext.Current.Response.Flush()
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            HttpContext.Current.Response.SuppressContent = True


        Catch ex As Exception
            Using jr As New JsonTextWriter(sw)
                With jr
                    '.CloseOutput = False
                    .WriteStartObject()
                    .WritePropertyName("response")
                    .WriteValue(ex.ToString)
                    .WriteEndObject()
                    '.WriteEnd()
                    '.CloseOutput = False
                End With
                strOut = sw.ToString
            End Using


            HttpContext.Current.Response.Clear()
            HttpContext.Current.Response.ContentType = "application/json; charset=utf-8"
            HttpContext.Current.Response.Write(strOut)
            HttpContext.Current.Response.Flush()
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            HttpContext.Current.Response.SuppressContent = True
        End Try

    End Function

    <WebMethod(),
    ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json, XmlSerializeString:=False)>
    Public Function FnGetScoreByMobilePhone(ByVal sMobilePhone As String,
                                            ByVal RequestID As String, ByVal UserID As String) As String
        Dim sb As New StringBuilder
        Dim sw As New IO.StringWriter(sb)
        Dim strOut As String = String.Empty

        Dim Httprequest As HttpWebRequest
        Dim response As HttpWebResponse
        Dim reader As StreamReader
        Dim rawresponse As String = ""

        Dim sResult As String = ""
        Dim rScore As String = ""
        Dim ProviderId As String = ""

        Dim oDs As String ' New DataSet
        Dim oLoad As New ClsPay

        sRequestID = Trim(RequestID.ToString)
        Try
            If sMobilePhone <> "" Then
                'Get Detail Provider 
                Dim oDd As New DataSet
                oDd = oLoad.GetDetailProvider(sMobilePhone)

                Dim UrlToken As String = " "
                Dim _WsUserName As String = ""
                Dim _WsPassword As String = ""

                UrlToken = oDd.Tables(0).Rows(0).Item(6)
                _WsUserName = oDd.Tables(0).Rows(0).Item(1)
                _WsPassword = oDd.Tables(0).Rows(0).Item(2)
                ProviderId = oDd.Tables(0).Rows(0).Item(0)

                'sResult = LoadScoreByMobilephone(sMobilePhone)

                If UserID = _sUserRequest Then
                    sResult = LoadScoreByMobilephone(sMobilePhone)
                End If

                If UserID = _sUserRequest And sResult <> "" Then
                    oDs = oLoad.InsertRequestLog(RequestID, sMobilePhone, sResult, UserID)
                Else

                    If UrlToken <> "" And ProviderId <> "" Then

                        'Get Token 
                        sToken = getTokenByProvider(ProviderId, UrlToken, _WsUserName, _WsPassword) ' getToken() ' get from database

                        'Get Encrypted MSISDN
                        Dim sUrlEncrypted As String = ""
                        sUrlEncrypted = oDd.Tables(0).Rows(0).Item(4)
                        sEncrypted = getEncryptedByProvider(sUrlEncrypted, sMobilePhone) ' validasi number

                        'Request Score
                        Dim sUrlGetScore As String = ""
                        sUrlGetScore = oDd.Tables(0).Rows(0).Item(7)

                        sEncryptedScore = sendRequestScoreByProvider(sUrlGetScore, Trim(sEncrypted.ToString), Trim(RequestID.ToString), Trim(sMobilePhone.ToString), UserID)
                        If Len(sEncryptedScore) < 100 Then
                            sResult = sEncryptedScore
                        Else
                            'Decrypt Score 

                            Dim sUrlDecrypted As String = ""
                            sUrlDecrypted = oDd.Tables(0).Rows(0).Item(5)
                            sResult = decryptScoreByProvider(sUrlDecrypted, sEncryptedScore)
                        End If
                    Else
                        If UserID = _sUserRequest Then
                            sResult = " "
                        Else
                            sResult = "UnRegistered Provider "
                        End If

                    End If
                    'sResult = "UnRegistered Provider "
                    oDs = oLoad.InsertRequestLog(RequestID, sMobilePhone, sResult, UserID)
                End If

            Else
                sResult = ""
            End If

            Using jr As New JsonTextWriter(sw)
                With jr
                    '.CloseOutput = False
                    .WriteStartObject()
                    .WritePropertyName("response")
                    .WriteValue(sResult)
                    .WriteEndObject()
                    '.WriteEnd()
                    '.CloseOutput = False
                End With
                strOut = sw.ToString
            End Using

            HttpContext.Current.Response.Clear()
            HttpContext.Current.Response.ContentType = "application/json; charset=utf-8"
            HttpContext.Current.Response.Write(strOut)
            HttpContext.Current.Response.Flush()
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            HttpContext.Current.Response.SuppressContent = True


        Catch ex As Exception
            Using jr As New JsonTextWriter(sw)
                With jr
                    '.CloseOutput = False
                    .WriteStartObject()
                    .WritePropertyName("response")
                    .WriteValue(ex.ToString)
                    .WriteEndObject()
                    '.WriteEnd()
                    '.CloseOutput = False
                End With
                strOut = sw.ToString
            End Using


            HttpContext.Current.Response.Clear()
            HttpContext.Current.Response.ContentType = "application/json; charset=utf-8"
            HttpContext.Current.Response.Write(strOut)
            HttpContext.Current.Response.Flush()
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            HttpContext.Current.Response.SuppressContent = True

        End Try

    End Function

    <WebMethod()>
    Function LoadScoreByMobilephone(ByVal sMobilephone As String) As String
        Dim sScore As String
        Dim rawresponse As String = ""
        Dim sResult As String = ""
        Dim sTable As String = ""

        Dim oDs As New DataSet
        Dim oLoad As New ClsPay
        'Dim nCol As Byte
        Dim nCol, nRow, x As Integer
        Dim i, j As Integer

        oDs = oLoad.LoadScoreByMobilephone(sMobilephone)
        nCol = oDs.Tables(0).Columns.Count - 1
        nRow = oDs.Tables(0).Rows.Count - 1
        'sScore = oDs.Tables(0).Rows(0).Item(0).ToString
        If oDs.Tables(0).Rows.Count <> 0 Then
            sScore = oDs.Tables(0).Rows(0).Item(0).ToString
        Else
            sScore = ""
        End If
        'dt.Rows.Count > 0

        Return sScore
    End Function

    <WebMethod()>
    Function LoadDetailApplicationNo(ByVal ApplicationNo As String)
        Try

            Dim rawresponse As String = ""
            Dim sResult As String = ""
            Dim sTable As String = ""

            Dim oDs As New DataSet
            Dim oLoad As New ClsPay
            'Dim nCol As Byte
            Dim nCol, nRow, x As Integer
            Dim i, j As Integer

            oDs = oLoad.LoadDetailApplicationNo(ApplicationNo)
            nCol = oDs.Tables(0).Columns.Count - 1
            nRow = oDs.Tables(0).Rows.Count - 1
            ''ApplicationNo CUST_NAME MobilePhone Score RequestDate


            sTable = sTable & " <table cellpadding='0' cellspacing='0' border='0' class='display' id='example'> "
            sTable = sTable & " <thead><tr>" &
                "<TD field='row_num' > No.</TD>" &
                    "<TD field='ApplicationNo' > ApplicationNo</TD>" &
                    "<TD field='CUST_NAME' > Customer Name</TD>" &
                    "<TD field='MobilePhone' > MobilePhone</TD>" &
                    "<TD field='Score' > Score</TD>" &
                    "<TD field='RequestDate' > Request Date</TD>" &
                    "<TD field='RequestDate' >  Current MobilePhone</TD>" &
                "<TD field='Other_app_no' > Action  </TD>" &
                    "<TD field='Other_app_no' > Other App No </TD>" &
                    "<TD field='Other_cust_name' > Other Cust Name</TD>" &
                    " </tr></thead>"


            With oDs.Tables(0)
                'sTable += "<table  class='tablegrid' cellSpacing='1' cellPadding='2'  border='1' width='95%'  >"
                sTable = sTable & "<tbody> "
                For x = 0 To nRow
                    sTable += "<tr>"
                    'sTable += "<td>" & .Rows(x).Item(8) & "</td>"
                    sTable += "<td>" & x + 1 & "</td>"
                    sTable += "<td>" & .Rows(x).Item(0) & "</td>"
                    sTable += "<td>" & .Rows(x).Item(1) & "</td>"
                    sTable += "<td>" & .Rows(x).Item(2) & "</td>"
                    sTable += "<td>" & .Rows(x).Item(3) & "</td>"
                    sTable += "<td>" & .Rows(x).Item(4).ToString() & "</td>"
                    sTable += "<td>" & .Rows(x).Item(8).ToString() & "</td>"

                    If .Rows(x).Item(3) = "-" Then
                        sTable += "<td>  <a  style='cursor:pointer'  onclick=""callWebService('" & .Rows(x).Item(8).ToString & "','" & .Rows(x).Item(0).ToString & "')"" id='btnGetScore'><font color='Blue'> <strong> Get Score </strong></font> </a></td> "
                    ElseIf (.Rows(x).Item(3) <> "-" And nRow = 0) Or (.Rows(x).Item(3) <> "-" And 0 = x) Then  '(.Rows(x).Item(3) <> "-" And nRow = x) Or (.Rows(x).Item(3) <> "-" And nRow = 0) Then
                        sTable += "<td>  <a  style='cursor:pointer'  onclick=""callWebService('" & .Rows(x).Item(8).ToString & "','" & .Rows(x).Item(0).ToString & "')"" id='btnGetScore'><font color='Blue'> <strong> Get New Score </strong></font> </a></td> "
                    Else
                        sTable += "<td>&nbsp; </td>"
                    End If

                    sTable += "<td>" & .Rows(x).Item(6) & "</td>"
                    sTable += "<td>" & .Rows(x).Item(7) & "</td>"
                    sTable += "</tr>"
                Next
                sTable = sTable & "</tbody>"
                'sTable += "</table>"
            End With

            Dim Script As String = ""
            Dim xStr As String = ""

            sTable += " </table>"
            sTable += "<input type = 'button'  onclick='PrintDiv();' value='Print' />"

            Script = "$(document).ready(Function() {" &
                        xStr &
              "})"
            sResult = Script & "__" & sTable

            Return sResult

        Catch ex As Exception
            Return ex.Message
        End Try

    End Function
    'decryptScoreByProvider(sUrlDecrypted

    Function decryptScoreByProvider(ByVal sUrlDecrypted As String, ByVal sEncryptedScore As String) As String
        Try


            Dim Httprequest As HttpWebRequest
            Dim response As HttpWebResponse
            Dim reader As StreamReader
            Dim rawresponse As String = ""

            'Get  Encrypted 
            Dim urlWebsrvices As String = sUrlDecrypted
            Httprequest = DirectCast(WebRequest.Create(urlWebsrvices), HttpWebRequest)
            Httprequest.ContentType = "application/json"
            Httprequest.Method = "POST"

            'request.TransferEncoding = True
            Httprequest.AllowAutoRedirect = True
            Httprequest.Proxy.Credentials = CredentialCache.DefaultCredentials
            Httprequest.UseDefaultCredentials = True
            Httprequest.Accept = "application/json"
            Httprequest.KeepAlive = True
            Httprequest.ProtocolVersion = HttpVersion.Version10

            Dim requestd As New sDecryptScore With {
            .result_decrypt = sEncryptedScore
        }


            Dim postdata As String = JsonConvert.SerializeObject(requestd)
            Dim requestWriter As StreamWriter = New StreamWriter(Httprequest.GetRequestStream())
            requestWriter.Write(postdata)
            requestWriter.Close()

            response = DirectCast(Httprequest.GetResponse(), HttpWebResponse)
            reader = New StreamReader(response.GetResponseStream())
            rawresponse = reader.ReadToEnd()


            Dim sResult As String = ""

            Dim ser As JObject = JObject.Parse(rawresponse)
            Dim data As List(Of JToken) = ser.Children().ToList
            For Each item As JProperty In data
                item.CreateReader()
                Select Case item.Name
                    Case "response"
                        sResult = item.Value
                End Select
            Next

            Return sResult

        Catch ex As Exception
            Return "Get Decrypt : " & ex.Message
        End Try



    End Function


    Function decryptScore(ByVal sEncryptedScore As String) As String
        Try


            Dim Httprequest As HttpWebRequest
            Dim response As HttpWebResponse
            Dim reader As StreamReader
            Dim rawresponse As String = ""

            'Get  Encrypted 
            Dim urlWebsrvices As String = _UrlDecrypted
            Httprequest = DirectCast(WebRequest.Create(urlWebsrvices), HttpWebRequest)
            Httprequest.ContentType = "application/json"
            Httprequest.Method = "POST"

            'request.TransferEncoding = True
            Httprequest.AllowAutoRedirect = True
            Httprequest.Proxy.Credentials = CredentialCache.DefaultCredentials
            Httprequest.UseDefaultCredentials = True
            Httprequest.Accept = "application/json"
            Httprequest.KeepAlive = True
            Httprequest.ProtocolVersion = HttpVersion.Version10

            Dim requestd As New sDecryptScore With {
            .result_decrypt = sEncryptedScore
        }


            Dim postdata As String = JsonConvert.SerializeObject(requestd)
            Dim requestWriter As StreamWriter = New StreamWriter(Httprequest.GetRequestStream())
            requestWriter.Write(postdata)
            requestWriter.Close()

            response = DirectCast(Httprequest.GetResponse(), HttpWebResponse)
            reader = New StreamReader(response.GetResponseStream())
            rawresponse = reader.ReadToEnd()


            Dim sResult As String = ""

            Dim ser As JObject = JObject.Parse(rawresponse)
            Dim data As List(Of JToken) = ser.Children().ToList
            For Each item As JProperty In data
                item.CreateReader()
                Select Case item.Name
                    Case "response"
                        sResult = item.Value
                End Select
            Next

            Return sResult

        Catch ex As Exception
            Return "Get Decrypt : " & ex.Message
        End Try



    End Function

    Function sendRequestScoreByProvider(ByVal sUrlGetScore As String, ByVal sEncrypted As String,
                              ByVal ApplicationNo As String, ByVal MobilePhone As String,
                              ByVal usrReq As String) As String

        Dim Httprequest As HttpWebRequest
        Dim response As HttpWebResponse
        Dim reader As StreamReader
        Dim rawresponse As String = ""

        'just playing around with Newtonsoft.Json  
        Dim sb As New StringBuilder
        Dim sw As New IO.StringWriter(sb)
        Dim strOut As String = String.Empty

        Dim sResult As String = ""

        ServicePointManager.DefaultConnectionLimit = 100
        ServicePointManager.MaxServicePointIdleTime = 5000

        ServicePointManager.Expect100Continue = True
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls ' SecurityProtocolType.Tls12
        ServicePointManager.SecurityProtocol = DirectCast(3072, SecurityProtocolType)
        Dim client = New WebClient
        client.Encoding = System.Text.Encoding.GetEncoding("ISO-8859-1")

        Try


            'Get  Encrypted 
            Dim urlWebsrvices As String = sUrlGetScore
            Httprequest = DirectCast(WebRequest.Create(urlWebsrvices), HttpWebRequest)
            Httprequest.ContentType = "application/json"
            Httprequest.Method = "POST"
            'Httprequest.Headers.Add("Authorization", "Bearer " + sToken)
            Httprequest.Headers("Authorization") = "Bearer " & Trim(sToken)

            'request.TransferEncoding = True
            Httprequest.AllowAutoRedirect = True
            Httprequest.Proxy.Credentials = CredentialCache.DefaultCredentials
            Httprequest.UseDefaultCredentials = True
            Httprequest.Accept = "application/json"
            Httprequest.KeepAlive = True
            Httprequest.ProtocolVersion = HttpVersion.Version10

            Dim requestd As New sGetscore With {
                .requested_msisdn = sEncrypted,
                .external_source_id = sRequestID 'GetRandomPass()
            }


            Dim postdata As String = JsonConvert.SerializeObject(requestd)
            Dim requestWriter As StreamWriter = New StreamWriter(Httprequest.GetRequestStream())
            requestWriter.Write(postdata)
            requestWriter.Close()

            response = DirectCast(Httprequest.GetResponse(), HttpWebResponse)
            reader = New StreamReader(response.GetResponseStream())
            'rawresponse = reader.ReadToEnd()
            'score 
            Dim jsonData As String = reader.ReadToEnd()
            reader.Close()

            ' insert jsonData  to table  
            Dim oDs As String ' New DataSet
            Dim oLoad As New ClsPay
            oDs = oLoad.InsertResponseLog(ApplicationNo, MobilePhone, usrReq, jsonData)


            Dim ser As JObject = JObject.Parse(jsonData)
            Dim data As List(Of JToken) = ser.Children().ToList
            For Each item As JProperty In data
                item.CreateReader()
                Select Case item.Name
                    Case "data"
                        For Each msg As JProperty In item.Values
                            If msg.Name = "score" Then
                                sResult = msg.Value
                            End If
                            'sResult = msg("access_token")
                        Next
                End Select
            Next

            Return sResult

        Catch ex As WebException

            Using xresponse As WebResponse = ex.Response
                Dim httpResponse As HttpWebResponse = CType(xresponse, HttpWebResponse)
                'Console.WriteLine("Error code: {0}", httpResponse.StatusCode)

                Using data As Stream = xresponse.GetResponseStream()
                    Dim text As String = New StreamReader(data).ReadToEnd()
                    'Console.WriteLine(text)
                    'sResult = text
                    Dim ser As JObject = JObject.Parse(text)
                    Dim datax As List(Of JToken) = ser.Children().ToList
                    For Each item As JProperty In datax
                        item.CreateReader()
                        Select Case item.Name
                            Case "message"
                                sResult = item.Value
                        End Select
                    Next

                End Using
            End Using

            Return sResult

            'Using jr As New JsonTextWriter(sw)
            '    With jr
            '        '.CloseOutput = False
            '        .WriteStartObject()
            '        .WritePropertyName("response")
            '        .WriteValue(ex.Response)
            '        .WriteEndObject()
            '        '.WriteEnd()
            '        '.CloseOutput = False
            '    End With
            '    sResult = sw.ToString
            'End Using

            'HttpContext.Current.Response.Clear()
            'HttpContext.Current.Response.ContentType = "application/json; charset=utf-8"
            'HttpContext.Current.Response.Write(sResult)
            'HttpContext.Current.Response.Flush()
            'HttpContext.Current.ApplicationInstance.CompleteRequest()
            'HttpContext.Current.Response.SuppressContent = True
        End Try

    End Function


    Function sendRequestScore(ByVal sEncrypted As String,
                              ByVal ApplicationNo As String, ByVal MobilePhone As String,
                              ByVal usrReq As String) As String

        Dim Httprequest As HttpWebRequest
        Dim response As HttpWebResponse
        Dim reader As StreamReader
        Dim rawresponse As String = ""

        'just playing around with Newtonsoft.Json  
        Dim sb As New StringBuilder
        Dim sw As New IO.StringWriter(sb)
        Dim strOut As String = String.Empty

        Dim sResult As String = ""

        ServicePointManager.DefaultConnectionLimit = 100
        ServicePointManager.MaxServicePointIdleTime = 5000

        ServicePointManager.Expect100Continue = True
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls ' SecurityProtocolType.Tls12
        ServicePointManager.SecurityProtocol = DirectCast(3072, SecurityProtocolType)
        Dim client = New WebClient
        client.Encoding = System.Text.Encoding.GetEncoding("ISO-8859-1")

        Try


            'Get  Encrypted 
            Dim urlWebsrvices As String = _UrlGetScore
            Httprequest = DirectCast(WebRequest.Create(urlWebsrvices), HttpWebRequest)
            Httprequest.ContentType = "application/json"
            Httprequest.Method = "POST"
            'Httprequest.Headers.Add("Authorization", "Bearer " + sToken)
            Httprequest.Headers("Authorization") = "Bearer " & Trim(sToken)

            'request.TransferEncoding = True
            Httprequest.AllowAutoRedirect = True
            Httprequest.Proxy.Credentials = CredentialCache.DefaultCredentials
            Httprequest.UseDefaultCredentials = True
            Httprequest.Accept = "application/json"
            Httprequest.KeepAlive = True
            Httprequest.ProtocolVersion = HttpVersion.Version10

            Dim requestd As New sGetscore With {
                .requested_msisdn = sEncrypted,
                .external_source_id = sRequestID 'GetRandomPass()
            }


            Dim postdata As String = JsonConvert.SerializeObject(requestd)
            Dim requestWriter As StreamWriter = New StreamWriter(Httprequest.GetRequestStream())
            requestWriter.Write(postdata)
            requestWriter.Close()

            response = DirectCast(Httprequest.GetResponse(), HttpWebResponse)
            reader = New StreamReader(response.GetResponseStream())
            'rawresponse = reader.ReadToEnd()
            'score 
            Dim jsonData As String = reader.ReadToEnd()
            reader.Close()

            ' insert jsonData  to table  
            Dim oDs As String ' New DataSet
            Dim oLoad As New ClsPay
            oDs = oLoad.InsertResponseLog(ApplicationNo, MobilePhone, usrReq, jsonData)


            Dim ser As JObject = JObject.Parse(jsonData)
            Dim data As List(Of JToken) = ser.Children().ToList
            For Each item As JProperty In data
                item.CreateReader()
                Select Case item.Name
                    Case "data"
                        For Each msg As JProperty In item.Values
                            If msg.Name = "score" Then
                                sResult = msg.Value
                            End If
                            'sResult = msg("access_token")
                        Next
                End Select
            Next

            Return sResult

        Catch ex As WebException

            Using xresponse As WebResponse = ex.Response
                Dim httpResponse As HttpWebResponse = CType(xresponse, HttpWebResponse)
                'Console.WriteLine("Error code: {0}", httpResponse.StatusCode)

                Using data As Stream = xresponse.GetResponseStream()
                    Dim text As String = New StreamReader(data).ReadToEnd()
                    'Console.WriteLine(text)
                    'sResult = text
                    Dim ser As JObject = JObject.Parse(text)
                    Dim datax As List(Of JToken) = ser.Children().ToList
                    For Each item As JProperty In datax
                        item.CreateReader()
                        Select Case item.Name
                            Case "message"
                                sResult = item.Value
                        End Select
                    Next

                End Using
            End Using

            Return sResult

            'Using jr As New JsonTextWriter(sw)
            '    With jr
            '        '.CloseOutput = False
            '        .WriteStartObject()
            '        .WritePropertyName("response")
            '        .WriteValue(ex.Response)
            '        .WriteEndObject()
            '        '.WriteEnd()
            '        '.CloseOutput = False
            '    End With
            '    sResult = sw.ToString
            'End Using

            'HttpContext.Current.Response.Clear()
            'HttpContext.Current.Response.ContentType = "application/json; charset=utf-8"
            'HttpContext.Current.Response.Write(sResult)
            'HttpContext.Current.Response.Flush()
            'HttpContext.Current.ApplicationInstance.CompleteRequest()
            'HttpContext.Current.Response.SuppressContent = True
        End Try

    End Function


    Function getEncryptedByProvider(ByVal _UrlEncrypted As String, ByVal MobilePhone As String) As String

        Dim Httprequest As HttpWebRequest
        Dim response As HttpWebResponse
        Dim reader As StreamReader
        Dim rawresponse As String = ""

        Dim sResult As String = ""

        Try

            'Get  Encrypted 
            Dim urlWebsrvices As String = _UrlEncrypted
            Httprequest = DirectCast(WebRequest.Create(urlWebsrvices), HttpWebRequest)
            Httprequest.ContentType = "application/json"
            Httprequest.Method = "POST"

            'request.TransferEncoding = True
            Httprequest.AllowAutoRedirect = True
            Httprequest.Proxy.Credentials = CredentialCache.DefaultCredentials
            Httprequest.UseDefaultCredentials = True
            Httprequest.Accept = "application/json"
            Httprequest.KeepAlive = True
            Httprequest.ProtocolVersion = HttpVersion.Version10

            Dim requestd As New sMobilePhone With {
            .phone_no = MobilePhone
        }


            Dim postdata As String = JsonConvert.SerializeObject(requestd)
            Dim requestWriter As StreamWriter = New StreamWriter(Httprequest.GetRequestStream())
            requestWriter.Write(postdata)
            requestWriter.Close()

            response = DirectCast(Httprequest.GetResponse(), HttpWebResponse)
            reader = New StreamReader(response.GetResponseStream())
            rawresponse = reader.ReadToEnd()



            Dim ser As JObject = JObject.Parse(rawresponse)
            Dim data As List(Of JToken) = ser.Children().ToList
            For Each item As JProperty In data
                item.CreateReader()
                Select Case item.Name
                    Case "response"
                        sResult = item.Value
                End Select
            Next

            Return sResult

        Catch ex As WebException

            Using xresponse As WebResponse = ex.Response
                Dim httpResponse As HttpWebResponse = CType(xresponse, HttpWebResponse)
                'Console.WriteLine("Error code: {0}", httpResponse.StatusCode)

                Using data As Stream = xresponse.GetResponseStream()
                    Dim text As String = New StreamReader(data).ReadToEnd()
                    'Console.WriteLine(text)
                    'sResult = text
                    Dim ser As JObject = JObject.Parse(text)
                    Dim datax As List(Of JToken) = ser.Children().ToList
                    For Each item As JProperty In datax
                        item.CreateReader()
                        Select Case item.Name
                            Case "message"
                                sResult = item.Value
                        End Select
                    Next

                End Using
            End Using

            Return sResult

            'Catch ex As Exception
            '    Return "Get Encrypted : " & ex.Message
        End Try

    End Function

    Function getEncrypted(ByVal MobilePhone As String) As String

        Dim Httprequest As HttpWebRequest
        Dim response As HttpWebResponse
        Dim reader As StreamReader
        Dim rawresponse As String = ""

        Dim sResult As String = ""

        Try

            'Get  Encrypted 
            Dim urlWebsrvices As String = _UrlEncrypted
            Httprequest = DirectCast(WebRequest.Create(urlWebsrvices), HttpWebRequest)
            Httprequest.ContentType = "application/json"
            Httprequest.Method = "POST"

            'request.TransferEncoding = True
            Httprequest.AllowAutoRedirect = True
            Httprequest.Proxy.Credentials = CredentialCache.DefaultCredentials
            Httprequest.UseDefaultCredentials = True
            Httprequest.Accept = "application/json"
            Httprequest.KeepAlive = True
            Httprequest.ProtocolVersion = HttpVersion.Version10

            Dim requestd As New sMobilePhone With {
                .phone_no = MobilePhone
            }


            Dim postdata As String = JsonConvert.SerializeObject(requestd)
            Dim requestWriter As StreamWriter = New StreamWriter(Httprequest.GetRequestStream())
            requestWriter.Write(postdata)
            requestWriter.Close()

            response = DirectCast(Httprequest.GetResponse(), HttpWebResponse)
            reader = New StreamReader(response.GetResponseStream())
            rawresponse = reader.ReadToEnd()



            Dim ser As JObject = JObject.Parse(rawresponse)
            Dim data As List(Of JToken) = ser.Children().ToList
            For Each item As JProperty In data
                item.CreateReader()
                Select Case item.Name
                    Case "response"
                        sResult = item.Value
                End Select
            Next

            Return sResult

        Catch ex As WebException

            Using xresponse As WebResponse = ex.Response
                Dim httpResponse As HttpWebResponse = CType(xresponse, HttpWebResponse)
                'Console.WriteLine("Error code: {0}", httpResponse.StatusCode)

                Using data As Stream = xresponse.GetResponseStream()
                    Dim text As String = New StreamReader(data).ReadToEnd()
                    'Console.WriteLine(text)
                    'sResult = text
                    Dim ser As JObject = JObject.Parse(text)
                    Dim datax As List(Of JToken) = ser.Children().ToList
                    For Each item As JProperty In datax
                        item.CreateReader()
                        Select Case item.Name
                            Case "message"
                                sResult = item.Value
                        End Select
                    Next

                End Using
            End Using

            Return sResult

            'Catch ex As Exception
            '    Return "Get Encrypted : " & ex.Message
        End Try

    End Function


    Function getToken() As String
        Dim Httprequest As HttpWebRequest
        Dim response As HttpWebResponse
        Dim reader As StreamReader
        Dim rawresponse As String = ""

        Dim sResult As String = ""

        Try
            Dim oDs As String ' New DataSet
            Dim oLoad As New ClsPay
            oDs = oLoad.GetCurrentToken

            If oDs <> "" Then
                sResult = oDs
            Else

                'Get Token 
                Dim urlWebsrvices As String = _UrlToken
                Httprequest = DirectCast(WebRequest.Create(urlWebsrvices), HttpWebRequest)
                Httprequest.ContentType = "application/json"
                Httprequest.Method = "POST"
                'request.TransferEncoding = True
                Httprequest.AllowAutoRedirect = True
                Httprequest.Proxy.Credentials = CredentialCache.DefaultCredentials
                Httprequest.UseDefaultCredentials = True
                Httprequest.Accept = "application/json"
                Httprequest.KeepAlive = True
                Httprequest.ProtocolVersion = HttpVersion.Version10

                Dim requestd As New sProfile

                Dim postdata As String = JsonConvert.SerializeObject(requestd)
                Dim requestWriter As StreamWriter = New StreamWriter(Httprequest.GetRequestStream())
                requestWriter.Write(postdata)
                requestWriter.Close()

                response = DirectCast(Httprequest.GetResponse(), HttpWebResponse)
                reader = New StreamReader(response.GetResponseStream())
                Dim jsonData As String = reader.ReadToEnd()
                reader.Close()

                Dim ser As JObject = JObject.Parse(jsonData)
                Dim data As List(Of JToken) = ser.Children().ToList
                For Each item As JProperty In data
                    item.CreateReader()
                    Select Case item.Name
                        Case "data"
                            For Each msg As JProperty In item.Values
                                If msg.Name = "access_token" Then
                                    sResult = msg.Value
                                    oDs = oLoad.InsertCurrentToken(sResult)
                                End If
                                'sResult = msg("access_token")
                            Next
                    End Select
                Next

            End If

            Return sResult

        Catch ex As Exception
            Return "Get Token : " & ex.Message
        End Try

    End Function

    Function getTokenByProvider(ByVal ProviderId As String, ByVal UrlToken As String,
                                ByVal WsUserName As String, ByVal WsPassword As String) As String
        Dim Httprequest As HttpWebRequest
        Dim response As HttpWebResponse
        Dim reader As StreamReader
        Dim rawresponse As String = ""

        Dim sResult As String = ""

        Try
            Dim oDs As String ' New DataSet
            Dim oLoad As New ClsPay
            oDs = oLoad.GetCurrentTokenByProvider(ProviderId)

            If oDs <> "" Then
                sResult = oDs
            Else

                'Get Token 
                Dim urlWebsrvices As String = UrlToken
                Httprequest = DirectCast(WebRequest.Create(urlWebsrvices), HttpWebRequest)
                Httprequest.ContentType = "application/json"
                Httprequest.Method = "POST"
                'request.TransferEncoding = True
                Httprequest.AllowAutoRedirect = True
                Httprequest.Proxy.Credentials = CredentialCache.DefaultCredentials
                Httprequest.UseDefaultCredentials = True
                Httprequest.Accept = "application/json"
                Httprequest.KeepAlive = True
                Httprequest.ProtocolVersion = HttpVersion.Version10


                Dim requestd As New sProfile With {
                    .user_name = WsUserName,
                    .password = WsPassword}


                Dim postdata As String = JsonConvert.SerializeObject(requestd)
                Dim requestWriter As StreamWriter = New StreamWriter(Httprequest.GetRequestStream())
                requestWriter.Write(postdata)
                requestWriter.Close()

                response = DirectCast(Httprequest.GetResponse(), HttpWebResponse)
                reader = New StreamReader(response.GetResponseStream())
                Dim jsonData As String = reader.ReadToEnd()
                reader.Close()

                Dim ser As JObject = JObject.Parse(jsonData)
                Dim data As List(Of JToken) = ser.Children().ToList
                For Each item As JProperty In data
                    item.CreateReader()
                    Select Case item.Name
                        Case "data"
                            For Each msg As JProperty In item.Values
                                If msg.Name = "access_token" Then
                                    sResult = msg.Value
                                    oDs = oLoad.InsertCurrentTokenByProviderID(sResult, ProviderId)
                                End If
                                'sResult = msg("access_token")
                            Next
                    End Select
                Next

            End If

            Return sResult

        Catch ex As Exception
            Return "Get Token : " & ex.Message
        End Try

    End Function

    Public Class MyRequestData
        Public Property sMobilePhone As String = ""
        Public Property RequestID As String = ""
        Public Property UserID As String = ""

    End Class

    Public Class sProfile

        ' Private _WsUserName As String = ConfigurationManager.AppSettings("sUsername")
        'Private _WsPassword As String = ConfigurationManager.AppSettings("sPassword")
        Public Property user_name As String = "" ' _WsUserName
        Public Property password As String = "" ' _WsPassword
    End Class

    Public Class sMobilePhone
        Public Property phone_no As String = ""
    End Class

    Public Class sGetscore
        Private _ClientCode As String = ConfigurationManager.AppSettings("sClientCode")

        Public Property client_code As String = _ClientCode 'sClientCode "buana_indonesia"
        Public Property requested_msisdn As String = ""
        Public Property external_source_id As String = ""
    End Class

    Public Class sDecryptScore
        Public Property result_decrypt As String = ""
    End Class
    Public Class TokenClass
        Public Property access_token As String = ""
    End Class
    Protected Function GetRandomPass() As String
        Dim pass As String = String.Empty
        Dim AllowedChars() As String = {"ABCDEFGHJKLMNPQRSTWXYZ", "abcdefghjklmnpqrstwxyz", "0123456789"}
        Dim rnd = New Random()
        While pass.Length <10
            Dim rndSet As Integer = rnd.Next(0, AllowedChars.Length)
            pass &= AllowedChars(rndSet).Substring(rnd.Next(0, AllowedChars(rndSet).Length), 1)
        End While
        Return pass
    End Function
End Class