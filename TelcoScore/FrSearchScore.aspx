<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FrSearchScore.aspx.vb" Inherits="TelcoScore.FrSearchScore" %>

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
     var sApplicationNo;
     function search(){
         sApplicationNo = document.getElementById("search-form").value;
            //callWebService(sNik); 
         LoadDetailApplicationNo(sApplicationNo);
         }

     function LoadDetailApplicationNo(sApplicationNo) {
         //var sUserId; 
         //sUserId = document.getElementById("txtUserID").value;

       if (sApplicationNo === "") {
                alert("please input the ApplicationNo");
                return false;
         }
           $('#dvLoader').show(); 
         var Scrip;
         var dataPassed = new Object(); 
         dataPassed.ApplicationNo = sApplicationNo;   
          $.ajax({       
                           type: "Post",
                           url: "WsGetScore.asmx/LoadDetailApplicationNo",   
                           data:$.toJSON(dataPassed), 
                           contentType: "application/json; charset=utf-8",
                           datatype: "json", 
                           success: function (msg) {
                              //divDataList.innerHTML= msg.d;// Scrip[1];
                               $('#dvLoader').hide();
                               Scrip = msg.d.split("__");
                               $("#DivQ").html(Scrip[1]); 
                               oTable = $('#example').dataTable({
                                   "bJQueryUI": true
                                   , "sPaginationType": "full_numbers"
                                   , "ordering": true 
                                   , "bSort": true
                                   , "oLanguage": { "sEmptyTable": "<font color='red'> Application No is Not Found</font>" }
                               });  
                               
                           },
                          error: function(XMLHttpRequest, textStatus, errorThrown) {
                              //alert(XMLHttpRequest.responseText);
                              divDataList.innerHTML = XMLHttpRequest.responseText;
                              //alert("error");
                                 } 
                            });
     }

 
   function  callWebService(MobilePhone , sApplicationNo ) { 
           // alert(sApplicationNo);
            var sUserId; 
            sUserId = document.getElementById("txtUserID").value;

          //if (MobilePhone === "") {
           //     alert("please input the debitur HP(628xxxxxxxxxxx)");
           //     return false;
           //}

            var Scrip;
            var dataPassed = new Object();
            var d = new Date().toISOString().slice(0, 10);
            var rndid = d+ Math.random().toString(36).substring(2, 15) //+ Math.random().toString(36).substring(2, 15);
           

            dataPassed.sMobilePhone = MobilePhone;  
            dataPassed.RequestID = sApplicationNo; //rndid.toUpperCase(); 
            dataPassed.UserID = sUserId;

                       $.ajax({       
                           type: "Post",
                           url: "WsGetScore.asmx/FnGetScoreByMobilePhone",   
                           data:$.toJSON(dataPassed), 
                           contentType: "application/json; charset=utf-8",
                           datatype: "json", 
                           success: function (msg) {
                                 // alert(msg.d);
                                // Scrip=msg.d.split("__");
                               //divDataList.innerHTML = msg.d;// Scrip[1];
                               LoadDetailApplicationNo(sApplicationNo);                                 
                           },

                          error: function(XMLHttpRequest, textStatus, errorThrown) {
                              //alert(XMLHttpRequest.responseText);
                              divDataList.innerHTML = XMLHttpRequest.responseText;
                              //alert("error");
                                 } 
                            });
                 }
                                  
        
     function PrintDiv() {
            var contents = document.getElementById("DivQ").innerHTML;
            var frame1 = document.createElement('iframe');
            frame1.name = "frame1";
            frame1.style.position = "absolute";
            frame1.style.top = "-1000000px";
            document.body.appendChild(frame1);
            var frameDoc = frame1.contentWindow ? frame1.contentWindow : frame1.contentDocument.document ? frame1.contentDocument.document : frame1.contentDocument;
            frameDoc.document.open();
            frameDoc.document.write('<html><head><title> Telco Score of ' + sApplicationNo + '</title>');
            frameDoc.document.write('</head><body>');
            frameDoc.document.write(contents);
            frameDoc.document.write('</body></html>');
            frameDoc.document.close();
            setTimeout(function () {
                window.frames["frame1"].focus();
                window.frames["frame1"].print();
                document.body.removeChild(frame1);
            }, 500);
            return false;
     }

     //$(function(){
     //   $('#printOut').click(function(e){
     //       e.preventDefault();
     //       var w = window.open();
     //       var printOne = $('.DivQHead').html();
     //       var printTwo = $('.DivQ').html();
     //       w.document.write('<html><head><title>Copy Printed</title></head><body><h1>Copy Printed</h1><hr />' + printOne + '<hr />' + printTwo) + '</body></html>';
     //       w.window.print();
     //       w.document.close();
     //       return false;
     //   });
     //});

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

</head>
 <body MS_POSITIONING="GridLayout" style ="background-color:#848683" > 
 			
<form class="search-form" name="suyb"  method="get">  
<%--onsubmit="search()"   --%>
  
 <div>
 <style>
        h2{font-family: Palatino, ‘Palatino Linotype’, serif; top:1%;left:35%;position:fixed }
</style>
<style>
.heading {
	font-size: 30px;
	text-align: center;
    font-family: Palatino, ‘Palatino Linotype’, serif; 
}

</style>

<h1 style="color:white;" class="heading"   >Search Debitur's Telco Score (By Application No)</h1>
</div>

 <TABLE id="TableHeader" cellSpacing="100" cellPadding="0" 
 width="100%"  border="0">
<tr align=center><td align=center >
<span>
    <input type="text" id="search-form" name="q" 
        placeholder="Please Entry The Application No ..." required maxlength="20"  />   
    <button type="button" onclick ="search()" id="btnSearch">Search</button> 
 
</span> 
</td></tr>
</table>
<%--<div id="divDataList"></div>--%>
   
    <!-- datagrid -->
<link rel="stylesheet" type="text/css" href="css/demo_page.css" />
<link rel="stylesheet" type="text/css" href="css/demo_table_jui.css" />
<link rel="stylesheet" type="text/css" href="css/jquery-ui-1.8.4.custom.css" />
<script type="text/javascript" src="css/jquery.dataTables.js"></script>
<!-- end of datagrid -->  
    <div id="DivQHead" style="border:1px solid gray; width:93%; margin-bottom: 1em; padding: 10px">
         <br />
        <div id="DivQ" class="tabcontent"></div>  
    </div>


   

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