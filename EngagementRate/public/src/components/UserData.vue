<template>
    <div>
        <nav class="navbar">
            <h1>Let's see the engagement rate</h1>
        </nav>
        <div class="main">
            <div class="inputs">
                <input class="input" type="text" v-model="userName" placeholder="Username .." />
                <button class="btn" @click="Init()">Show Data</button>
            </div>
        </div>
    </div>
    <div class="user__notFound" v-if="notFound">
        <span>User Not Found</span>
    </div>
    <div class="user">
         <img src="../assets/bugfathercom.jpg" /> 
         <img v-bind:src="PHOTO_URL" /> 
          <img src="../assets/Circles-menu-3.gif" />
        <div class="user__loading" v-if="loading">
            <img src="../assets/Circles-menu-3.gif" />
        </div>
        <div v-show="user.username" v-if="!loading">
            <div class="user__info">
                <img class="user__avatar" v-bind:src="'../assets/avatar/' + user.username + '.jpeg'" />
                <div class="user__info-bio">
                    <div class="user__info-item">{{ user.username }}</div>
                    <div class="user__info-item">{{ user.full_name }}</div>
                    <div class="user__info-item user__info--txt">{{ user.biography }}</div>
                </div>
            </div>
            <button @click="getUserPosts(user.posts_Count)"></button>
            <div class="user__data">
                <div class="user__data-item">
                    <div class="user__data-item user__item--h">
                        <div>follower</div>
                        <div>{{ user.follower }}</div>
                    </div>
                    <div class="user__data-item user__item--h">
                        <div>Avarage likes</div>
                        <div>{{ user_posts.avg_likes }}</div>
                    </div>
                </div>
                <div class="user__data-item">
                    <div class="user__data-item user__item--h">
                        <div>following</div>
                        <div>{{ user.following }}</div>
                    </div>
                    <div class="user__data-item user__item--h">
                        <div>Average Comments</div>
                        <div>{{ user_posts.avg_comments }}</div>
                    </div>
                </div>
                <div class="user__data-item">
                    <div class="user__data-item user__item--h">
                        <div>posts</div>
                        <div>{{ user.posts_Count }}</div>
                    </div>
                    <div class="user__data-item user__item--h">
                        <div>Average View Video</div>
                        <div>{{ user_posts.avg_view }}</div>
                    </div>
                </div>
            </div>
        </div>

    </div>

</template>

<script lang="js">
import { defineComponent } from 'vue';

import axios from 'axios'
const variables = {
    API_URL: 'https://localhost:5001/api',
    PHOTO_URL: 'https://localhost:5001/img/',
    INASTAGRAM_API_URL: 'https://i.instagram.com/api/v1/'
}
let _user_posts = [];
let usernametest = 'bugfathercom.jpg';
//import variables from '../variables'
let _PHOTO_URL= '~/public/avatar/' + usernametest;

export default defineComponent({
    data() {
        return {
            userName: '',
            instagramUserData: {},
            user: {},
            user_posts: _user_posts,
            loading: false,
            notFound: false,
            avg_likes: 0,
            //usernametest: 'bugfathercom.jpg',
            PHOTO_URL: _PHOTO_URL.toString()
        }
    },
    methods: {
        Init() {
            this.loading = true;
            this.notFound = false;
            let _this = this;
            _user_posts = [];
            _user_posts.sum_likes = 0;
            _user_posts.sum_video_view = 0;
            _user_posts.sum_comments = 0;
            _user_posts.video_count = 0;
            this.user = {};
            this.user_posts = [];
            this.GetUserByUserName(this.userName).then(function (result) {
                let instagramUserData = result;
                _this.user.full_name = instagramUserData.full_name;
                _this.user.biography = instagramUserData.biography;
                _this.user.is_private = instagramUserData.is_private;
                _this.user.follower = instagramUserData.edge_followed_by.count;
                _this.user.following = instagramUserData.edge_follow.count;
                _this.user.posts_Count = instagramUserData.edge_owner_to_timeline_media.count;
                
                _this.user.username = instagramUserData.username;
                _this.user.Id = instagramUserData.id;
               

                _this.init_end_cursor = instagramUserData.edge_owner_to_timeline_media.page_info.end_cursor;
                _this.init_has_next_page = instagramUserData.edge_owner_to_timeline_media.page_info.has_next_page;
                _this.ProcessPostsArray(instagramUserData.edge_owner_to_timeline_media.edges).then(function () {
                    _this.getUserPosts(instagramUserData.edge_owner_to_timeline_media.count).then((post) => {
                        if (post.length == _this.user.posts_Count) {
                            _user_posts.avg_likes = (_user_posts.sum_likes / _this.user.posts_Count) | 0;
                            _user_posts.avg_comments = (_user_posts.sum_comments / _this.user.posts_Count) | 0;
                            _user_posts.avg_view = (_user_posts.sum_video_view / _user_posts.video_count) | 0;
                            _this.user_posts = _user_posts;
                        }
                        _this.loading = false;
                    });
                     _this.user.profileImage = _this.GetAvatar(instagramUserData.profile_pic_url,instagramUserData.username);
                })
            });

        },
        GetUserByUserName(userName) {
            return new Promise(resolve => {
                let url = variables.API_URL + '/UserData/UserInfo/' + userName;
                let result = this.CreateRequest(url);
                resolve(result);
            });
        },
        CreateRequest(url) {
            return new Promise(resolve => {
                axios.get(url)
                    .then(response => {
                        resolve(response.data.data.user);
                    })
                    .catch(error => {
                        this.notFound = true;
                        console.log(error.toString());
                    })
            })

        },
        CreateRequestWithData(url, urlparam) {            
            return new Promise(resolve => {
                axios.get(url + urlparam)
                    .then(response => {
                        resolve(response.data.data.user);
                    })
                    .catch(error => {
                        this.notFound = true;
                        console.log(error.toString());
                    })
            })

        },
        ProcessPostsArray(postArray) {
            let user_posts = [];
            let _this = this;


            return new Promise(resolve => {
                postArray.forEach(function (item) {
                    let node = item.node;

                    let post = {};
                    post.id = node.id;
                    post.__typename = node.__typename;
                    // GraphVideo - GraphVideo - GraphSidecar

                    if (Object.keys(node.shortcode).length !== 0) {
                        post.shortcode = node.shortcode;
                    } else {
                        post.shortcode = null;
                    }

                    post.dimensions = node.dimensions;
                    post.display_url = node.display_url;
                    post.is_video = node.is_video;
                    post.thumbnail_src = node.thumbnail_src;
                    post.thumbnail_resources = node.thumbnail_resources;

                    switch (node.__typename) {
                        case "GraphVideo":
                            post.video_url = node.video_url;
                            post.has_audio = node.has_audio;
                            post.video_view_count = node.video_view_count;
                            _user_posts.sum_video_view += post.video_view_count;
                            _user_posts.video_count++;
                            break;
                        case "GraphSidecar":
                            post.slides = _this.ProcessPostsSlides(node.edge_sidecar_to_children.edges);

                            break;

                    }

                    post.comments_disabled = node.comments_disabled;
                    // if(node.shortcode != undefined){

                    // }
                    if (node.edge_media_preview_like.count > 0) {
                        post.like_count = node.edge_media_preview_like.count;
                    } else {
                        post.like_count = 0;
                    }

                    post.comment_count = node.edge_media_to_comment.count;
                    post.caption = node.edge_media_to_caption.edges[0].node.text;

                    _user_posts.sum_likes += post.like_count;
                    _user_posts.sum_comments += post.comment_count;

                    user_posts.push(post);
                    _user_posts.push(post);
                    post = null;
                })

                console.log(_user_posts);
                resolve(_user_posts);
            });

        },
        ProcessPostsSlides(slides_array) {
            let final_slide_array = [];
            slides_array.forEach(function (item) {
                let node = item.node;
                let slide = {};
                slide.__typename = node.__typename;
                slide.id = node.id;
                if (node.shortcode != undefined) {
                    slide.shortcode = node.shortcode;
                } else {
                    slide.shortcode = null;
                }
                slide.dimensions = node.dimensions;
                slide.display_url = node.display_url;
                slide.is_video = node.is_video;


                if (node.__typename == "GraphVideo") {
                    slide.video_url = node.video_url;
                    slide.video_view_count = node.video_view_count;
                    slide.has_audio = node.has_audio;
                }
                final_slide_array = slide;
            })
            return final_slide_array;
        },
        getUserPosts(request_count) {
            let current_post_count = _user_posts.length;
            return new Promise(resolve => {
                if (this.init_has_next_page) {
                    if (request_count < current_post_count) {
                        resolve(_user_posts.slice(_user_posts, 0, request_count));
                    }
                    if (request_count > current_post_count) {
                        this.RunPostMaker(this.init_end_cursor, this.init_has_next_page, request_count)
                            .then((result) => {
                                console.log(result);
                                if (_user_posts.length > request_count) {
                                    resolve(_user_posts.splice(0, request_count));
                                } else {
                                    resolve(_user_posts);
                                }
                            });
                    }
                }
                else {
                    if (_user_posts.length > request_count) {
                        resolve(_user_posts.splice(0, request_count));
                    } else {
                        resolve(_user_posts);
                    }
                }
            });

        },
        RunPostMaker(end_cursor, has_next_page, request_count) {
            let _this = this;
            return new Promise(resolve => {
                if (has_next_page) {
                    if (_this.user_posts.length <= request_count) {
                        _this.FetchPosts(end_cursor).then(function (result) {
                            resolve(_this.RunPostMaker(result.end_cursor, result.has_next_page, request_count));
                        });

                    }
                }
                else {
                    resolve(has_next_page);
                }
            });

        },
        FetchPosts(end_cursor) {
            // $sample = 'https://www.instagram.com/graphql/query/?
            //     query_hash=396983faee97f4b49ccbe105b4daf7a0
            //     &variables=%7B%22
            //     id%22%3A%22
            //     528817151%22%2C%22
            //     first
            //     %22%3A12%2C%22
            //     after
            //     %22%3A%22
            //     QVFDcXdHX0tWd0FxUFI0MGJlcVZzdHV5Q3FrNHlQSDRhN3dGMlhkNHkzaHNYQ3hSU1hkdjcxb3B2Yklfb2VBcnBUNTJaTFB1blFFSUNzQ3YxeWdNWUF5Tw%3D%3D
            //     %22%7D';
            let _this = this;
            return new Promise(resolve => {
                let query_hash = '396983faee97f4b49ccbe105b4daf7a0';
                let variables = {};
                variables.id = JSON.parse(JSON.stringify(_this.user)).Id;
                variables.first = 12;
                variables.after = end_cursor;
                let query_string_array = [];
                query_string_array.query_hash = query_hash;
                query_string_array.variables = variables;

                _this.UserPostsInfo(query_string_array).then(function (result) {
                    let posts = result.edge_owner_to_timeline_media.edges;
                    _this.ProcessPostsArray(posts).then((res) => {
                        let has_next_page = result.edge_owner_to_timeline_media.page_info.has_next_page;
                        let end_cursor = result.edge_owner_to_timeline_media.page_info.end_cursor;
                        console.log(res);
                        resolve({ posts: res, has_next_page: has_next_page, end_cursor: end_cursor });
                    });
                });

            });



        },
        UserPostsInfo(query_string_array) {
            let param = {
                query_hash: query_string_array.query_hash,
                variables: encodeURIComponent(JSON.stringify(query_string_array.variables))
            }
            return new Promise(resolve => {
                let url = variables.API_URL + '/UserData/PostsInfo/';
                let urlparam = param.query_hash + '&variables=' + param.variables;
                this.CreateRequestWithData(url, urlparam).then(function (result) {
                    resolve(result);
                });

            });
        },
        GetAvatar(url_img,username) {
            let _this = this;
            let param = {
                    UrlImage : url_img.toString(),
                    UserName: username
                };
            return new Promise(resolve => {
                let url = variables.API_URL + '/UserData/GetAvatar' ;
                _this.CreateRequestPost(url,param).then((result) => {
                    resolve(result);
                });
            });
        },
        CreateRequestPost(url, param) {            
            return new Promise(resolve => {
                axios.post(url , param)
                    .then(response => {
                        resolve(response.data.data.user);
                    })
                    .catch(error => {
                        console.log(error.toString());
                    })
            })

        },
        GetUserData() {
            // const headers = {
            //     'Content-Type': 'application/json',
            //     "Access-Control-Allow-Origin": "https://localhost:5001/"
            // }
            axios.get(variables.API_URL + '/UserData?userName=' + this.User)
                .then((response) => {

                    this.userName = response.data;
                    this.userdata = response.data.data;
                    this.pic = this.userdata.user.profile_pic_url;
                    this.fullname = this.userdata.user.full_name;
                    this.biography = this.userdata.user.biography;
                    this.folowers = this.userdata.user.edge_followed_by.count;
                    this.folowing = this.userdata.user.edge_follow.count;
                    this.posts = this.userdata.user.edge_owner_to_timeline_media.count;
                    let likesCount = 0;
                    let likes = 0;
                    response.data.data.user.edge_owner_to_timeline_media.edges.forEach(function (item, index) {
                        likes += item.node.edge_liked_by.count;
                        likesCount = index;
                    }, this.avg_likes = likes / likesCount)
                })
        }
    },
});
</script>

<style>
@font-face {
    font-family: "Merienda";
    src: local("Merienda"), url(https://fonts.googleapis.com/css2?family=Merienda&display=swap) format("truetype");
}

.navbar {
    background-color: #2c3639;
    padding: 16px;
    color: white;
}

.main {
    margin-top: 30vh;
}

.inputs {
    display: flex;
    align-items: stretch;
    justify-content: center;
}

.input {
    border: 1px solid #DCD7C9;
    height: 30px;
    padding: 10px;
    border-top-left-radius: 5px;
    border-bottom-left-radius: 5px;
}

input[type="text"].input:hover {
    border: 1px solid #A27B5C;
}

input[type="text"].input:focus {
    border: none;
}

input[type="text"].input:active {
    border: 1px solid #A27B5C;
}

.btn {
    background: #A27B5C;
    display: inline-flex;
    align-items: center;
    padding-right: 10px;
    padding-left: 10px;
    border-bottom-right-radius: 5px;
    border-top-right-radius: 5px;
    cursor: pointer;
    color: white;
    text-decoration: none;
    border: none;
}

.user {
    margin-right: auto;
    margin-left: auto;
    max-width: 500px;
    margin-top: 50px;
    height: 100%;
    padding-left: 16px;
    padding-right: 16px;
}

.user__loading {
    text-align: center;
    display: flex;
    align-items: center;
    justify-content: center;
    height: 100%;
}

.user__notFound {
    text-align: center;
    display: flex;
    align-items: center;
    justify-content: center;
    height: 100%;
    font-size: 46px;
    margin-top: 50px;
}

.user__avatar {
    width: 77px;
    height: 77px;
    border-radius: 50%;
}

.user__info {
    display: flex;
    margin-bottom: 30px;
    justify-content: space-between;
}

.user__info-bio {
    display: flex;
    justify-content: flex-start;
    flex-direction: column;
    align-items: flex-end;
    text-align: right;
}

.user__info-item {
    margin-bottom: 10px;
}

.user__info--txt {
    white-space: pre-line;
}

.user__data {
    display: flex;
    justify-content: space-around;
}

.user__data-item {
    display: flex;
    flex-direction: column;
    align-items: center;
    margin: 10px;
    justify-content: space-between;
}

.user__item--h {
    height: 40px;
}
</style>