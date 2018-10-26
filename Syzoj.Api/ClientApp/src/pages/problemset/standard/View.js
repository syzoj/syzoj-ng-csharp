import React, { Component } from 'react'
import { request } from '../../../util'
import ViewModel from '../../../components/ViewModel'

export default class View extends Component {
    constructor(props) {
        super(props)
        this.state = { loading: true }
    }
    
    componentWillMount() {
        request(`/api/problemset-standard/fc474d6c-0890-48e6-9305-ca8995918832/view/${this.props.match.params.id}`).then(data => {
            this.setState({ loading: false, data: data })
        })
    }

    render() {
        if(this.state.loading) {
            return <p>Loading</p>
        } else {
            return <ViewModel model={this.state.data} />
        }
    }
}