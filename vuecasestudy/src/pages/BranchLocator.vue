<template>
  <div class="text-center q-mt-md">
    <q-avatar class="q-mt-md" size="100px" square>
      <img :src="'/icons/CartIcon.png'" />
    </q-avatar>
    <div class="text-h6">Find 3 closest Branches To:</div>
    <div>
      <q-input
        class="q-ma-lg text-h5"
        placeholder="enter current address"
        id="address"
        v-model="state.address"
      />
      <br />
    </div>
    <q-btn label="Find 3" @click="genMap" class="q-mb-md" style="width: 30vw" />
    <div
      style="height: 50vh; width: 90vw; margin-left: 5vw; border: solid"
      ref="mapRef"
      v-show="state.showmap === true"
    ></div>
  </div>
</template>
<script>
import { ref, reactive } from 'vue'
import { fetcher as get } from '../utils/apiutil'

export default {
  name: 'BranchLocator',
  setup() {
    const mapRef = ref(null)
    let state = reactive({
      status: '',
      address: '',
      showmap: false,
    })

    const getLatLon = async (address) => {
      try {
        let url = `https://api.tomtom.com/search/2/geocode/${address}.json?key=ziYrOVnHLvzfYQ9tdhc2rCopnd0CjkVq`
        let response = await fetch(url)
        let payload = await response.json()
        return payload.results[0].position
      } catch (err) {
        state.status = err.message
      }
    }

    const getClosestBranches = async (lat, lon) => {
      try {
        let response = await get(`branch/${lat}/${lon}`)
        console.log(response)
        return response
      } catch (err) {
        state.status = err.message
      }
      return []
    }
    const genMap = async () => {
      try {
        state.showmap = true
        const tt = window.tt
        let url = `https://api.tomtom.com/search/2/geocode/${state.address}.json?key=ziYrOVnHLvzfYQ9tdhc2rCopnd0CjkVq`
        let response = await fetch(url)
        let payload = await response.json()
        let lat = payload.results[0].position.lat
        let lon = payload.results[0].position.lon
        let map = tt.map({
          key: 'ziYrOVnHLvzfYQ9tdhc2rCopnd0CjkVq',
          container: mapRef.value,
          source: 'vector/1/basic-main',
          center: [lon, lat],
          zoom: 8,
        })
        map.addControl(new tt.FullscreenControl())
        map.addControl(new tt.NavigationControl())

        let location = await getLatLon(state.address)
        console.log(location)
        let stores = await getClosestBranches(location.lat, location.lon)

        stores.forEach((store) => {
          let marker = new tt.Marker()
            .setLngLat([Number(store.longitude), Number(store.latitude)])
            .addTo(map)

          let popup = new tt.Popup({ offset: 25 })
          popup.setHTML(`
  <div id="popup">Branch#: ${store.id}</div>
  <div>${store.street}, ${store.city}</div>
  <div>${store.distance.toFixed(2)} km</div>
`)
          marker.setPopup(popup)
        })
      } catch (err) {
        state.status = err.message
      }
    }
    return {
      mapRef,
      state,
      genMap,
      getLatLon,
      getClosestBranches,
    }
  },
}
</script>
