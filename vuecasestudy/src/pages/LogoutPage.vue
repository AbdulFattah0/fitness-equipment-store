<template>
  <div class="text-h5 text-center q-mt-xl text-primary">User now logged out</div>
</template>

<script>
import { onMounted } from 'vue'

export default {
  name: 'LogoutPage',
  setup() {
    onMounted(() => {
      sessionStorage.clear()
    })
  },
}
</script>
