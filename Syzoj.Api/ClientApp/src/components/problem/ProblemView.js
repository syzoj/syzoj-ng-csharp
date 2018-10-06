import React, { Component } from 'react'

const loadProblemView = problemType => import(`./${problemType}/View`)

export class ProblemView extends Component
{
    constructor(props)
    {
        super(props)
        this.state = {}
    }

    componentWillMount()
    {
        loadProblemView(this.props.problemTypeName).then(problemType => {
            this.setState({problemType: problemType})
        })
    }

    render()
    {
        if(!this.state.problemType)
            return <p>Loading...</p>
        else {
            var ProblemType = this.state.problemType.View
            return <ProblemType {...this.props} />
        }
    }
}