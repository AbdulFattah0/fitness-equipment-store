<template>
  <div>
    <div class="row justify-center q-mt-md">
      <q-avatar size="80px" style="border-radius: 50%">
        <img :src="`/img/BrandLogo.png`" />
      </q-avatar>
    </div>
    <div class="text-h4 text-center q-mt-md q-mb-md text-primary">Login</div>
    <div class="text-title2 text-center text-negative text-bold q-mt-sm" v-if="state.status">
      {{ state.status }}
    </div>

    <q-card class="q-ma-md q-pa-md">
      <q-form ref="myForm" class="q-gutter-md" greedy @submit="login" @reset="resetFields">
        <q-input
          outlined
          placeholder="Email Input"
          id="email"
          v-model="state.email"
          :rules="[isRequired, isValidEmail]"
        />
        <q-input
          outlined
          placeholder="Password Input"
          id="password"
          v-model="state.password"
          :type="'password'"
          :rules="[isRequired]"
        />
        <q-btn label="Login" type="submit" />
        <q-btn label="Reset" type="reset" />
      </q-form>
    </q-card>
  </div>
</template>

<script>
import { poster } from 'src/utils/apiutil.js'
import { reactive } from 'vue'
import { useRouter, useRoute } from 'vue-router'

export default {
  setup() {
    const route = useRoute()
    const router = useRouter()
    const isValidEmail = (val) => {
      const emailPattern =
        /^(?=[a-zA-Z0-9@._%+-]{6,254}$)[a-zA-Z0-9._%+-]{1,64}@(?:[a-zA-Z0-9-]{1,63}\.){1,8}[a-zA-Z]{2,63}$/
      return emailPattern.test(val) || 'Invalid email'
    }

    let state = reactive({
      email: '',
      password: '',
      status: '',
    })

    const isRequired = (val) => {
      return !!val || 'field is required'
    }
    const login = async () => {
      let customerHelper = {
        email: state.email,
        password: state.password,
      }
      try {
        let payload = await poster('Customer/login', customerHelper)

        if (payload.token && !payload.token.toLowerCase().includes('failed')) {
          sessionStorage.setItem('customer', JSON.stringify(payload))
          state.status = 'Login successful!' //
          route.query.nextUrl
            ? router.push({ path: route.query.nextUrl })
            : router.push({ path: '/' })
        } else {
          state.status = payload.token || 'Login failed'
        }
      } catch (err) {
        state.status = err.message
      }
    }
    const resetFields = () => {
      state.example = ''
      state.status = ''
    }
    return {
      state,
      login,
      isRequired,
      resetFields,
      isValidEmail,
    }
  },
}
</script>
