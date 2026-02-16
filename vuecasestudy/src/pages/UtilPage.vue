<template>
  <div class="text-center">
    <div class="text-h2 q-mt-lg">Utility</div>
  </div>

  <q-btn class="q-ma-sm" color="white" text-color="black" label="Load Tables" @click="loadTables" />
  <div class="status q-mt-md text-subtitle2 text-negative" text-color="red">
    {{ state.status }}
  </div>
</template>

<script>
import { reactive } from 'vue'
import { fetcher } from '../utils/apiutil'
export default {
  setup() {
    let state = reactive({
      status: '',
    })

    const loadTables = async () => {
      try {
        state.status = 'resetting exercise tables ...'
        state.status = await fetcher(`Data`)
      } catch (err) {
        console.log(err)
        state.status = `Error has occured: ${err.message}`
      }
    }

    return {
      loadTables,
      state,
    }
  },
}
</script>
