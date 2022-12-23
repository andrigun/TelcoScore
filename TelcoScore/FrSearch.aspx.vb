Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Security.Cryptography

Public Class FrSearch
    Inherits System.Web.UI.Page


    Dim sUserID As String
    Dim sUserName As String
    Dim sUserEncrypted As String

    Const APP_ID As String = "973" ' request telco score

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '' Me.txtUserID.Value = "gundala"
        ''txtUserID.Value = System.Web.HttpContext.Current.User.Identity.Name
        Dim oUser As New clsUser

        sUserID = Replace(System.Web.HttpContext.Current.User.Identity.Name, "BDF\", "")

        ''sUserID = "andrigun"

        If sUserID <> "" Then
            'storeSessionInfo()
            'Response.Redirect("index.html")
            If oUser.getApplicationAccessRight(sUserID, APP_ID) = False Then
                Response.Redirect("msg.aspx", True)
            Else
                txtUserID.Value = sUserID.ToString
            End If

        Else
            ' error page redirect 
            Response.Redirect("msg.aspx", True)
        End If

        If oUser.getIsGroup(sUserID, "97301") <> True Then ' User Request
            Response.Redirect("msg.aspx", True)
        End If

    End Sub

End Class