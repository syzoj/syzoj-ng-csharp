import React, { Component } from 'react'
import ViewModel from '../components/ViewModel'
import { request } from '../util'
import config from '../config'

export default class SubmissionView extends Component {
    constructor(props) {
        super(props)
        this.state = { loading: true }
        request(`/api/problemset-standard/${config.defaultProblemsetId}/submission/${this.props.match.params.id}/view`).then(data => {
            this.setState({ loading: false, data: data })
        })
    }

    render() {
        if(this.state.loading) {
            return <p>Loading</p>
        } else {
            return <ViewModel model={this.state.data.Content} />
        }
    }
}