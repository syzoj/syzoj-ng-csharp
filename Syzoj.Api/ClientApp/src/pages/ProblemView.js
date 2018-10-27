import React, { Component } from 'react'
import { request } from '../util'
import ViewModel from '../components/ViewModel'
import config from '../config'

export default class ProblemView extends Component {
    constructor(props) {
        super(props)
        this.state = { loading: true }
        request(`/api/problemset-standard/${config.defaultProblemsetId}/view/${this.props.match.params.id}`).then(data => {
            this.setState({ loading: false, data: data })
        })
    }

    submit(token) {
        request(`/api/problemset-standard/${config.defaultProblemsetId}/submit`, 'POST', {
            EntryId: this.state.data.EntryId,
            Token: token
        }).then(id => alert(id))
    }

    render() {
        if(this.state.loading) {
            return <p>Loading</p>
        } else {
            return <ViewModel model={this.state.data.Content} onSubmit={(token) => this.submit(token)} />
        }
    }
}