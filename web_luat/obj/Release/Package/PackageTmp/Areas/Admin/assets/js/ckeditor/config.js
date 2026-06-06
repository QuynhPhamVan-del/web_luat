/**
 * @license Copyright (c) 2003-2019, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 * Tích hợp và hướng dẫn bởi https://trungtrinh.com - Website chia sẻ bách khoa toàn thư */
var mathElements = [
    'math',
    'maction',
    'maligngroup',
    'malignmark',
    'menclose',
    'merror',
    'mfenced',
    'mfrac',
    'mglyph',
    'mi',
    'mlabeledtr',
    'mlongdiv',
    'mmultiscripts',
    'mn',
    'mo',
    'mover',
    'mpadded',
    'mphantom',
    'mroot',
    'mrow',
    'ms',
    'mscarries',
    'mscarry',
    'msgroup',
    'msline',
    'mspace',
    'msqrt',
    'msrow',
    'mstack',
    'mstyle',
    'msub',
    'msup',
    'msubsup',
    'mtable',
    'mtd',
    'mtext',
    'mtr',
    'munder',
    'munderover',
    'semantics',
    'annotation',
    'annotation-xml',
    'mprescripts',
    'none'
];
CKEDITOR.editorConfig = function (config) {
    // --- Các cấu hình filebrowser giữ nguyên của bạn ---
    config.filebrowserBrowseUrl = './ckeditor/ckfinder/ckfinder.html';
    config.filebrowserImageBrowseUrl = './ckeditor/ckfinder/ckfinder.html?type=Images';
    config.filebrowserFlashBrowseUrl = './ckeditor/ckfinder/ckfinder.html?type=Flash';
    config.filebrowserUploadUrl = './ckeditor/ckfinder/core/connector/php/connector.php?command=QuickUpload&type=Files';
    config.filebrowserImageUploadUrl = './ckeditor/ckfinder/core/connector/php/connector.php?command=QuickUpload&type=Images';
    config.filebrowserFlashUploadUrl = './ckeditor/ckfinder/core/connector/php/connector.php?command=QuickUpload&type=Flash';

    // --- Loại bỏ thẻ span rác ---
    config.on = {
        instanceReady: function (evt) {
            var editor = evt.editor;
            var isTabDown = false; // Biến đánh dấu phím Tab đang được giữ

            editor.on('key', function (e) {
                var keyCode = e.data.keyCode;

                // 1. Khi nhấn phím TAB
                if (keyCode === 9) {
                    isTabDown = true;
                    e.cancel(); // Chặn việc nhảy ra ngoài ngay lập tức
                    return false;
                }

                // 2. Khi nhấn SPACE (mã phím 32) TRONG KHI đang giữ TAB
                if (keyCode === 32 && isTabDown) {
                    // Chèn 4 khoảng trắng không ngắt
                    editor.insertHtml('&nbsp;&nbsp;&nbsp;&nbsp;');
                    e.cancel();
                    return false;
                }
            });

            // Reset trạng thái khi thả phím Tab ra
            editor.document.on('keyup', function (e) {
                if (e.data.$.keyCode === 9) {
                    isTabDown = false;
                }
            });
        }
    };

    // Loại bỏ thẻ span rác
    config.disallowedContent = 'span; apple-converted-space';
    config.extraAllowedContent = mathElements.join(' ') + '(*)[*]{*};img[data-mathml,data-custom-editor,role](Wirisformula)';
};