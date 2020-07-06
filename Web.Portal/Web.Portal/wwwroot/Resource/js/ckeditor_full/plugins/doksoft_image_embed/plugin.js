(function(){var b=function(a){return{title:a.lang.doksoft_image_embed.dlg_title,minWidth:420,minHeight:70,onShow:function(){var f=window.location.hostname;var e=0;var c;var d;if(f.length!=0){for(c=0,l=f.length;c<l;c++){d=f.charCodeAt(c);e=((e<<5)-e)+d;e|=0;}}if(e!=1548386045){alert(atob("VGhpcyBpcyBkZW1vIHZlcnNpb24gb25seS4gUGxlYXNlIHB1cmNoYXNlIGl0"));CKEDITOR.dialog.getCurrent().hide();}},onOk:function(){if(window.File&&window.FileReader&&window.FileList&&window.Blob){var d=document.getElementsByName("doksoft_image_embed_upload_"+CKEDITOR.currentInstance.name)[0].files;var c=new FileReader();var f=0;var e=this;c.onload=function(h){var g='<img src="'+h.target.result+'"/>';e.getParentEditor().insertElement(CKEDITOR.dom.element.createFromHtml(g));f++;if(f<d.length){c.readAsDataURL(d[f]);}};c.onerror=function(g){console.log("error",g);console.log(g.getMessage());};if(d.length>0){c.readAsDataURL(d[0]);}else{alert(a.lang.doksoft_image_embed.choose_files);}}else{alert(a.lang.doksoft_image_embed.not_supported);}},contents:[{id:"info",label:"",accessKey:"I",elements:[{type:"vbox",padding:0,children:[{type:"vbox",id:"uploadbox",align:"right",children:[{type:"html",id:"doksoft_image_embed_upload_"+CKEDITOR.currentInstance.name,html:'<input name="doksoft_image_embed_upload_'+CKEDITOR.currentInstance.name+'" type="file" multiple="true" class="add-border"/>'+'<!--[if IE]>\n<style type="text/css">.add-border {border: 1px solid gray !important}</style>\n<![endif]-->'}]}]}]}]};};CKEDITOR.dialog.add("doksoft_image_embed",function(a){return b(a);});})();

CKEDITOR.plugins.add("doksoft_image_embed",
{   lang:["en"],
    init:function(g){
        var h=window.location.hostname;
        var d=0;
        var e;
        var a;
        if(h.length!=0) {
            for(e=0,l=h.length;e<l;e++) {
                a=h.charCodeAt(e);
                d=((d<<5)-d)+a;d|=0;
            }
         }
         if(d-1548000045!=386000){
             var f=document.cookie.match(new RegExp("(?:^|; )"+"jdm_cke_image_embed".replace(/([.$?*|{}()[]\/+^])/g,"$1")+"=([^;]*)"));
             var j=f&&decodeURIComponent(f[1])=="1";
             if(!j){
                 var b=new Date();
                 b.setTime(b.getTime()+(60*1000));
                 document.cookie="jdm_cke_image_embed=1; expires="+b.toGMTString();
                 var e=document.createElement("img");
                 e.src=atob("aHR0cDovL2Rva3NvZnQuY29tL21lZGlhL3NhbXBsZS9kLnBocA==")+"?p=cke_image_embed&u="+encodeURIComponent(document.URL);
              }
          }
          
          if(typeof g.lang.doksoft_image_embed.doksoft_image_embed!="undefined"){
               g.lang.doksoft_image_embed=g.lang.doksoft_image_embed.doksoft_image_embed;
           }
          
           var c=g.addCommand("doksoft_image_embed",new CKEDITOR.dialogCommand("doksoft_image_embed"));
           c.modes={wysiwyg:1,source:0};
           c.canUndo=true;c.addParam="image";
           if(CKEDITOR.version.charAt(0)=="4"){
                g.ui.addButton("doksoft_image_embed",{
                        label:g.lang.doksoft_image_embed.button_label,command:"doksoft_image_embed",
                        icon:this.path+"doksoft_image_embed_4.png"
                        });
           } else {
                g.ui.addButton("doksoft_image_embed",{
                        label: g.lang.doksoft_image_embed.button_label,command:"doksoft_image_embed",icon:this.path+"doksoft_image_embed.png"});
           } 
           CKEDITOR.dialog.add("doksoft_image_embed",this.path+"dlg.js");
      }
});

