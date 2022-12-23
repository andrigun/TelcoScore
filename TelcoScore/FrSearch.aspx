<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FrSearch.aspx.vb" Inherits="TelcoScore.FrSearch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
   <title>Search Debitur NIK Application</title>
    
    <%--<link rel="stylesheet" media="screen" href="Css/login.css" />--%>
     <link rel="stylesheet" media="screen" href="Css/style.css" />
    <link rel="stylesheet" media="screen" href="Css/Form.css" />
    
</head>

<script src="Scripts/jquery-1.5.1.js" type="text/javascript"></script>
<script src="Scripts/jquery.json-1.3.min.js"></script>
 <script type="text/javascript" > 
     function search(){
        var sNik;
         sNik = document.getElementById("search-form").value;
         callWebService(sNik); 
         }
         
        function  callWebService(sNik) { 
           //alert(sNik);
            var sUserId; 
            sUserId = document.getElementById("txtUserID").value;

            if (sNik === "") {
                alert("please input the debitur HP(628xxxxxxxxxxx)");
                return false;
            }
                 $('#dvLoader').show(); 
            var Scrip;
            var dataPassed = new Object();
            var d = new Date().toISOString().slice(0, 10);
            var res = d.replace(/-/g, "");
            var rndid = res+ Math.random().toString(36).substring(2, 6) //+ Math.random().toString(36).substring(2, 15);
           

            dataPassed.sMobilePhone = sNik;  
            dataPassed.RequestID = rndid.toUpperCase(); 
            dataPassed.UserID = sUserId;

                       $.ajax({       
                           type: "Post",
                           url: "WsGetScore.asmx/FnGetScoreByMobilePhoneInternal",   
                           data:$.toJSON(dataPassed), 
                           contentType: "application/json; charset=utf-8",
                           datatype: "json", 
                           success: function (msg) {
                                 // alert(msg.d);
                                // Scrip=msg.d.split("__");
                               $('#dvLoader').hide();
                                 divDataList.innerHTML= msg.d;// Scrip[1];
                                 
                           },
                          error: function(XMLHttpRequest, textStatus, errorThrown) {
                              //alert(XMLHttpRequest.responseText);
                              divDataList.innerHTML = XMLHttpRequest.responseText;
                              //alert("error");
                                 } 
                            });
                 }
                                  
        function isNumber(evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57 )) {
                return false;
            }
            if (window.event && window.event.keyCode === 13){
                return false;
            } 
        }
        
    </script> 
  
 <script type='text/javascript'> 
title = " Search Debitur's Telco Score ";
position = 0;
function scrolltitle() {
    document.title = title.substring(position, title.length) + title.substring(0, position); 
    position++;
    if (position > title.length) position = 0;
    titleScroll = window.setTimeout(scrolltitle,170);
}
     scrolltitle();


     function printDiv() {
         var sNik, sDetail;
         sNik = document.getElementById("search-form").value;
         sDetail =document.getElementById("divDataList").innerHTML ;  
         //sDetail  = sDetail.innerHTML; 
        
           if (sNik === "" ) {
                alert("please input the debitur Mobile Phone");
                return false;
            }
         if ( sDetail  === "" ) {
                alert("please click the search button");
                return false;
            }

            var divId ='divDataList';
            var content = document.getElementById(divId).innerHTML; 
            var mywindow = window.open('', 'Print', 'height=600,width=800');
         
            mywindow.document.write(content); 
            mywindow.document.close();
            mywindow.focus()
            mywindow.print();
            mywindow.close();
            return true;
        }   
        
</script>
<style type="text/css">
    @media print {
        header,footer { 
        display: none; 
        }
    }
    
@print{
    @page :footer {color: #fff }
    @page :header {color: #fff}
}
</style>

    <style>
.heading {
	font-size: 30px;
	text-align: center;
    font-family: Palatino, ‘Palatino Linotype’, serif; 
}

</style>

</head>
 <body MS_POSITIONING="GridLayout" style ="background-color:#848683" > 
 			
<form class="search-form" name="suyb"  method="get">  
<%--onsubmit="search()"   --%>
  
 <div>  <h1 style="color:white;" align="center" class="heading" >Search Debitur's Telco Score (By MobilePhone)</h1> 

</div>

 <TABLE id="TableHeader" cellSpacing="100" cellPadding="0"   width="100%"  border="0">
<tr align=center><td align=center >
<span>
    <input type="text" id="search-form" name="q" 
        placeholder="Please Entry The Debitur's MobilePhone ..." required
        maxlength="20"  onkeypress="return isNumber(event)" 
        />  
         <%--onkeypress="return isNumber(event)" --%>
          <%--onkeypress="return event.keyCode!=13"--%>
    <button type="button" onclick ="search()" id="btnSearch">Search</button> 
 
</span> 
</td></tr>
</table>
<div id="divDataList"></div>
   
    
<div id="dvLoader" style="display: none;"> 
    <div id="updateProgressBackgroundFilter" style="background-color: lightgrey;
    filter: alpha(opacity=40); opacity: 0.80; width: 100%; top: 0px; left: 0px; position: fixed;">
    <h2>Please Wait..</h2>
    </div>
    <div class="centered" style="font-family: Trebuchet MS;        
    filter: alpha(opacity=100); opacity: 1; font-size: small; position: absolute;" >
    <h2> on progress..</h2>
    <br /><br /> 
    <img  alt="Loading" src="css/img/cyborg2.gif"  height="200" width="200"/>
    </div>
</div>

    <input type="text" id="txtUserID" runat="server" style="display:none" />

</form> 

</body>
</html>