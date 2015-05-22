function Editable(elementid, fieldKey, fieldType, siteLanguageId, pageId) {
    $("#" + elementid).editable({
        mode: 'popup',
        type: fieldType,
        url: '/unigate/editable/edit/save/',
        ajaxOptions: {
            type: 'post'
        },
        params: {
            'FieldKey': fieldKey,
            'FieldType': fieldType,
            'SiteLanguageId': siteLanguageId,
            'PageId': pageId
        },
        pk: 1,
        send: 'always',
        title: 'Edit Field',
        success: function (data, config) {
            debugger;
            if (data && data.id) {  //record created, response like {"id": 2}
                //set pk
                $(this).editable('option', 'pk', data.id);
                //remove unsaved class
                $(this).removeClass('editable-unsaved');
                //show messages
                var msg = 'New user created! Now editables submit individually.';
                $('#msg').addClass('alert-success').removeClass('alert-error').html(msg).show();
                $('#save-btn').hide();
                $(this).off('save.newuser');
            } else if (data && data.errors) {
                //server-side validation error, response like {"errors": {"username": "username already exist"} }
                config.error.call(this, data.errors);
            }
        },
    });
}
