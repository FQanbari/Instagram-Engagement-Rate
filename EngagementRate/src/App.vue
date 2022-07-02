<template>
  <!--<img alt="Vue logo" src="./assets/logo.png">-->
  <UserData msg="Welcome to Your Vue.js App"/>
</template>

<script>
//import HelloWorld from './components/HelloWorld.vue'
import UserData from './components/UserData.vue'

export default {
  name: 'App',
  components: {
      //HelloWorld,
      UserData
  }
}
</script>

<style>

</style>
