﻿"use strict";

//elements
var conversation = $('.conversation');
var lastSentMessages = $('.messages--sent:last-child');
var lastReceivedMessages = $('.messages--received:last-child');
var textbar = $('.text-bar__field input');
var textForm = $('#form-message');
var user = $('#hidUserName');
var loadMoreContainer = $('#loadMoreContainer');

var scrollTop = $(window).scrollTop();

var Message = {
    cachedObj: {
        pageIndex: 1
    },
    currentText: '',
    init: function () {
        var base = this;
        base.initSignalR();

        conversation.html('');
        base.loadMessages(false);
        base.registerEvents();
    },
    initSignalR: function () {
        var chat = $.connection.chatHub;
        chat.client.receiveMessage = function (message) {
            var base = Message;
            if ($('#hidUserName').val() === message.SenderName) {
                base.createGroup(true);
                base.createMessage(true, message.Content);

                setTimeout(function () {
                    base.scrollDown();
                }, 50);
            } else {
                var isBottom = base.isBottom();

                base.createGroup(false);
                base.createMessage(false, message.Content);

                if (isBottom) {
                    setTimeout(function () {
                        base.scrollDown();
                    }, 50);
                }
            }
        };

        $.connection.hub.start().done(function () {
            var base = Message;
            textForm.on('submit', function () {
                event.preventDefault();
                base.saveText();
                if (base.currentText !== '') {
                    chat.server.sendMessage(localStorage.getItem('receiver'), base.currentText);
                }
            });
        });
    },
    saveText: function () {
        var base = this;
        base.currentText = textbar.val();
        textbar.val('');
    },
    createMessage: function (isSender, message) {
        var base = this;
        if (isSender) {
            lastSentMessages.append($('<div/>')
                .addClass('message')
                .text(message));
        } else {
            lastReceivedMessages.append($('<div/>')
                .addClass('message')
                .text(message));
        }
    },
    createGroup: function (isSender) {
        if (isSender) {
            if (conversation.html()) {
                if ($('.messages:last-child').hasClass('messages--received')) {
                    conversation.append($('<div/>')
                        .addClass('messages messages--sent'));
                    lastSentMessages = $('.messages--sent:last-child');
                }
            } else {
                conversation.append($('<div/>')
                    .addClass('messages messages--sent'));
                lastSentMessages = $('.messages--sent:last-child');
            }
        } else {
            if (conversation.html()) {
                if ($('.messages:last-child').hasClass('messages--sent')) {
                    conversation.append($('<div/>')
                        .addClass('messages messages--received'));
                    lastReceivedMessages = $('.messages--received:last-child');
                }
            } else {
                conversation.append($('<div/>')
                    .addClass('messages messages--received'));
                lastReceivedMessages = $('.messages--received:last-child');
            }
        }
    },
    scrollDown: function () {
        var base = this;
        //conversation.scrollTop(conversation[0].scrollHeight);
        conversation.stop().animate({
            scrollTop: conversation[0].scrollHeight
        }, 500);
    },
    isBottom: function () {
        var current = Math.round(conversation.scrollTop() + conversation.innerHeight(), 10);
        var total = Math.round(conversation[0].scrollHeight, 10);
        return total === current ? true : false;
    },
    loadMoreMessages: function (item) {
        var base = Message;
        if ($('#hidUserName').val() === item.SenderName) {
            if (loadMoreContainer.html()) {
                if (loadMoreContainer.find('.messages:last-child').hasClass('messages--received')) {
                    loadMoreContainer.append($('<div/>')
                        .addClass('messages messages--sent'));
                }
            } else {
                loadMoreContainer.append($('<div/>')
                    .addClass('messages messages--sent'));
            }

            loadMoreContainer.find('.messages--sent:last-child')
                .append($('<div/>')
                    .addClass('message')
                    .text(item.Content));
        } else {
            if (loadMoreContainer.html()) {
                if (loadMoreContainer.find('.messages:last-child').hasClass('messages--sent')) {
                    loadMoreContainer.append($('<div/>')
                        .addClass('messages messages--received'));
                }
            } else {
                loadMoreContainer.append($('<div/>')
                    .addClass('messages messages--received'));
            }

            loadMoreContainer.find('.messages--received:last-child')
                .append($('<div/>')
                    .addClass('message')
                    .text(item.Content));
        }
    },
    loadMessages: function (isloadMore = false) {
        var receiverStorage = localStorage.getItem('receiver');
        if (!receiverStorage) {
            $('.user-row').removeClass('user-message-active');
            var firstUser = $('.user-row:first-child');
            firstUser.addClass('user-message-active');
            localStorage.setItem('receiver', firstUser.data('user'));
        } else {
            $('.user-row').each(function () {
                var userName = $(this).data('user');
                if (userName === receiverStorage) {
                    $('.user-row').removeClass('user-message-active');
                    $(this).addClass('user-message-active');
                    return false;
                }
            });
        }

        receiverStorage = localStorage.getItem('receiver');
        var base = this;
        $.ajax({
            url: '/Home/GetMessages',
            type: 'GET',
            data: {
                receiver: receiverStorage,
                pageIndex: base.cachedObj.pageIndex++
            },
            success: function (response) {
                console.log(response);
                if (response.length > 0) {
                    var html = '';
                    $.each(response, function (idx, item) {
                        if (isloadMore === true) {
                            base.loadMoreMessages(item);
                        } else {
                            if ($('#hidUserName').val() === item.SenderName) {
                                base.createGroup(true);
                                base.createMessage(true, item.Content);
                            } else {
                                base.createGroup(false);
                                base.createMessage(false, item.Content);
                            }
                        }
                    });

                    if (isloadMore) {
                        var scrollarea = document.getElementById('conversation');
                        var lastScrollHeight = scrollarea.scrollHeight;
                        setTimeout(function () {
                            conversation.prepend(loadMoreContainer.html());
                            scrollarea.scrollTop += (scrollarea.scrollHeight - lastScrollHeight);
                        }, 200);
                    } else {
                        base.scrollDown();
                    }
                }
            }
        });
    },
    registerEvents: function () {
        var base = this;

        $('.user-row').on('click', function (e) {
            e.preventDefault();
            var receiver = $(this).data('user');
            if (receiver === localStorage.getItem('receiver')) {
                return false;
            }

            localStorage.setItem('receiver', receiver);
            console.log(localStorage.getItem('receiver'));

            base.cachedObj.pageIndex = 1;

            $('.user-row').removeClass('user-message-active');
            $(this).addClass('user-message-active');

            loadMoreContainer.html('');
            conversation.html('');
            base.loadMessages(false);
        });

        conversation.scroll(function () {
            if (conversation.scrollTop() === conversation.height() - conversation.height()) {
                base.loadMessages(true);
            }
        });
    }
};

var newMessage = Object.create(Message);
newMessage.init();
