import React, { Component } from 'react'
import { request } from '../../../util'

export default class View extends Component
{
    constructor(props) {
        super(props)
        this.state = {loading: true, data: null}
    }

    componentWillMount() {
        request(`/api/problem-standard/${this.props.match.params.id}/view`, 'GET', null).then(data => {
            this.setState({loading: false, data: data})
        })
    }

    render()
    {
        if(this.state.loading)
            return <p>Loading</p>
        else
            return <p>{JSON.stringify(this.state.data)}</p>
    }
}