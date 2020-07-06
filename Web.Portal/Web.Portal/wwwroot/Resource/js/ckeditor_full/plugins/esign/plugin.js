CKEDITOR.plugins.add( 'esign', {
    icons: 'esign',
    init: function( editor ) {
        editor.addCommand( 'insertESign', {
            exec: function( editor ) {
                editor.insertHtml( '<div class="esign" style="display:inline-block;"></div>' );
            }
        });    

        editor.ui.addButton('eSign', {
            label: 'Insert eSign',
            command: 'insertESign',
            toolbar: 'insert'
        });   
    }
});