<template>
	<v-app>
		<div class="g-container">
			<div class="g-grid-container">
				<div class="g-absolute-controls">
					<v-form 
						v-model="form" 
						ref="form"
					>
						<v-text-field
							label="Grid size"
							placeholder="A value between 2 & 20"
							v-model.number="interimMultiplier"
							hide-details
							max="20"
							min="2"
							type="number"
							outlined
							:rules="rules"
						/>
					</v-form>
					<v-btn
						@click="reset(interimMultiplier)"
						:disabled="!form || successSnackbar"
						icon
						large
					>
						<v-icon>mdi-reload</v-icon>
					</v-btn>
				</div>
				<div
					v-for="(row, x) in grid"
					class="g-grid-row"
					:key="`row${x}`"
					:style="{
						gridTemplateColumns: `repeat(${multiplier}, 1fr)`,
						height: `${(600 - (10 * multiplier)) / multiplier}px`
					}"
				>
					<div
						v-for="(entry, y) in row"
						:class="{ 'g-grid-entry': true, 'g-active': entry }"
						@click="successSnackbar ? null : alterEntry(x, y)"
						:key="`entry${x}${y}`"
						v-ripple
					></div>				
				</div>		
			</div>
		</div>
		<v-snackbar
			v-model="snackbar"
			color="red"
		>
			{{ snackbarText }}
		</v-snackbar>
		<v-snackbar
			v-model="successSnackbar"
			color="#66bb6a"
			timeout="5000"
		>
			🎉 You did it! Congratulations!
		</v-snackbar>
	</v-app>
</template>

<script>
import axios from 'axios';

export default {

	data() {
		return {
			snackbar: false,
			successSnackbar: false,
			form: true,
			multiplier: 5,
			interimMultiplier: 5,
			snackbarText: '',
			grid: [],
			rules: [
				v => !!v || 'Required',
				v => v <= 20 && v >= 2 || 'Required',
			],
		}
	},

	created() {
		// Initalise and load the current matrix data
		axios.get('/Gridgame/GetMatrix')
			.then(({ data }) => {
				this.grid = data;
				this.multiplier = data[0].length;
			})
			.catch((err) => this.handleErr(err));
	},

	methods: {
		// Errors will be displayed in the snackbar
		handleErr(err) {
			this.snackbar = true;
			this.snackbarText = err; 
		},
		// Alters the state of a given entry
		alterEntry(x, y){
			axios.post('/Gridgame/PostAlterEntry', { x, y })
				.then(({ data }) => {
					this.grid = data;

					// If they're all off, then let the user know they won & reset
					// after a suitable amount of congratulatory time (5s)
					if (data.flat().every((x) => !x)) {
						this.successSnackbar = true;

						setTimeout(() => {
							this.reset(this.multiplier);
						}, 5000);
					}
				})
				.catch((err) => this.handleErr(err));
		},
		// Resets the matrix to its default values
		reset(multiplier){
			axios.post('/Gridgame/PostReset', { multiplier })
				.then(({ data }) => {
					this.grid = data;
					this.multiplier = multiplier;
				})
				.catch((err) => this.handleErr(err));
		}
	}
};
</script>

<style lang="scss">
	@import './assets/global.scss';
</style>
