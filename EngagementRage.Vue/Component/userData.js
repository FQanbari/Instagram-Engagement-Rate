const userData = {
    template: '<h1>User Data Information</h1><input v-model="User" placeholder="edit me">    <button type="button" @click="getUserData()">Link</button> <div>{{userName}}</div>',
    data(){
        return{
            userName:{},
            userdata:this.userName,
            pic:'',
            fullname:'',
            biography: '',
            folowers:0,
            folowing:0,
            posts:0,
            avg_likes:0,
            User:''
        }
    },
    methods:{
        getUserData(){
            const headers = {
                'Content-Type': 'application/json',
                "Access-Control-Allow-Origin": "https://localhost:5001/"
              }
            axios.get(variables.API_URL+'/UserData?userName=' + this.User,{
                mode: 'no-cors',
                headers: headers
              })
            .then((response) => {
                this.userName = response.data;
                this.pic = this.userdata.user.profile_pic_url;
                this.fullname = this.userdata.user.full_name;
                this.biography = this.userdata.user.biography;
                this.folowers = this.userdata.user.edge_followed_by.count;
                this.folowing = this.userdata.user.edge_follow.count;
                this.posts = this.userdata.user.edge_owner_to_timeline_media.count;
                let likesCount = 0;
                let likes = 0;
                this.userdata.user.edge_owner_to_timeline_media.edges.forEach(index,item,function (item) {
                    likes += item.node.edge_liked_by;
                    likesCount = index;
                }).then(function(){
                    this.avg_likes = likes / likesCount;
                })
            })
        }
    },
    function getInput() {
        return new Promise(resolve => {
          const inputUI = new Vue(InputUI);
          
          inputUI.$once('submit', value => {
            inputUI.$destroy();
            inputUI.$el.remove();
            resolve(value);
          });
          
          inputUI.$mount();
          document.body.appendChild(inputUI.$el);
        });
      }
      
}